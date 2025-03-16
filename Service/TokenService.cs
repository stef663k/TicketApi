using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TicketApi.Models;
using TicketApi.Interface;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TicketApi.Data;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly DatabaseContext _context;
    private readonly SymmetricSecurityKey _securityKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _tokenExpiryMinutes;

    public TokenService(IConfiguration config, DatabaseContext context)
    {
        _config = config;
        _context = context;
        
        var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        _issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
        _audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        _tokenExpiryMinutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var expiry) ? expiry : 15;
    }

    private string GenerateJwtToken(User user, IEnumerable<Claim> claims)
    {
        var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_tokenExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateToken(User user) => GenerateJwtToken(user, new[] {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), // UserId included here
        new Claim(JwtRegisteredClaimNames.Name, user.Username),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    });

    public string GenerateAccessToken(User user) => GenerateJwtToken(user, new[] {
        new Claim(ClaimTypes.NameIdentifier, user.Username),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    });

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32]; // Increase entropy
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public bool ValidateRefreshToken(string refreshToken, Guid userId)
    {
        var storedToken = _context.RefreshTokens
            .FirstOrDefault(rt => rt.Token == refreshToken 
                               && rt.UserId == userId
                               && !rt.IsRevoked 
                               && rt.Expires > DateTime.UtcNow);
        
        return storedToken != null;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _securityKey,
            ValidateLifetime = false // Allow expired tokens
        };

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        
        if (securityToken is not JwtSecurityToken jwtSecurityToken || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    // For extracting UserId from token after login
    public Guid GetUserIdFromToken(string token)
    {
        var principal = GetPrincipalFromExpiredToken(token);
        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub); // Fetch the UserId
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId; // Return the UserId as Guid
        }
        throw new SecurityTokenException("UserId claim not found in token or invalid format");
    }

}
