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

    public TokenService(IConfiguration config, DatabaseContext context)
    {
        _config = config;
        _context = context;
    }

    public string GenerateToken(User user)
    {
        try
        {
            var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            var issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            var audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token generation failed: {ex}");
            throw;
        }
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Username),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public bool ValidateRefreshToken(string refreshToken, Guid userId)
    {
        var storedToken = _context.RefreshTokens
            .FirstOrDefault(rt => rt.Token == refreshToken 
                               && rt.UserId == userId
                               && !rt.IsRevoked);
        
        return storedToken != null && storedToken.Expires > DateTime.UtcNow;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),
            ValidateLifetime = false // Allow expired tokens
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(
            token, 
            tokenValidationParameters, 
            out SecurityToken securityToken);
        
        if (securityToken is not JwtSecurityToken jwtSecurityToken || 
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256, 
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}
