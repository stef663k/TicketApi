using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketApi.DTO;
using TicketApi.Service;
using System.Security.Claims;
using TicketApi.Data;
using AutoMapper;
using TicketApi.Models;
using TicketApi.Interface;

namespace TicketApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public AuthController(IAuthService authService, ITokenService tokenService, DatabaseContext context, IMapper mapper)
    {
        _authService = authService;
        _tokenService = tokenService;
        _context = context;
        _mapper = mapper;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            Console.WriteLine($"Model errors: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors))}");
            return BadRequest(ModelState);
        }
        
        var result = await _authService.LoginAsync(loginDto);
        
        Console.WriteLine($"Login result: {result != null}");
        
        if (result == null)
            return Unauthorized();
        
        return Ok(result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        Console.WriteLine($"Role recieved: {registerDto.Role}");
        var result = await _authService.RegisterAsync(registerDto);
        return result != null ? Ok(result) : BadRequest("Username already exists");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid token claims");
        }

        if (!_tokenService.ValidateRefreshToken(request.RefreshToken, userId))
            return Unauthorized("Invalid refresh token");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return Unauthorized("User not found");
        }

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = userId,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            User = _mapper.Map<UserResponseDTO>(user)
        });
    }
}

