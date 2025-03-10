using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketApi.Data;
using TicketApi.Models;
using TicketApi.Service;
using TicketApi.DTO;
using TicketApi.Interface;
using AutoMapper;
using BCrypt.Net;
using Microsoft.CodeAnalysis.Scripting;
using System.Security.Claims;

namespace TicketApi.Service;
public class AuthService : IAuthService
{
    private readonly DatabaseContext _context;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(
        DatabaseContext context,
        IPasswordHasher hasher,
        ITokenService tokenService,
        IMapper mapper)
    {
        _context = context;
        _hasher = hasher;
        _tokenService = tokenService;
        _mapper = mapper;
        _context.Database.EnsureCreated();
        Console.WriteLine($"Users in DB: {_context.Users.Count()}");
    }

    public async Task<AuthResponse?> LoginAsync(LoginDto loginDto)
    {
        try
        {
            Console.WriteLine($"Login attempt for: {loginDto.Username}");

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username.ToLower() == loginDto.Username.ToLower() &&
                    !u.IsDeleted);

            Console.WriteLine($"User found: {user != null}");

            if (user == null) return null;

            Console.WriteLine($"Password match: {_hasher.VerifyPassword(user.PasswordHash, loginDto.Password)}");

            Console.WriteLine($"[{DateTime.UtcNow}] Login attempt for: {loginDto.Username}");

            Console.WriteLine($"User found: {user.UserId} | Role: {user.Role} | Last Login: {user.LastLogin}");
            Console.WriteLine($"Stored hash: {user.PasswordHash}");

            if (!_hasher.VerifyPassword(user.PasswordHash, loginDto.Password))
            {
                Console.WriteLine("Password verification failed");
                return null;
            }

            Console.WriteLine("Password verified");
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Store refresh token
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.UserId,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = _mapper.Map<UserResponseDTO>(user)
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {DateTime.UtcNow}: {ex}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterDto registerDto)
    {
        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = _hasher.HashPassword(registerDto.Password),
            Role = registerDto.Role,
        };

        var hashResult = _hasher.HashPassword(registerDto.Password);
        user.PasswordHash = hashResult;

        Console.WriteLine($"Storing password hash: {hashResult?.Substring(0, 20)}...");

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            User = _mapper.Map<UserResponseDTO>(user)
        };
    }

    public async Task RevokeRefreshToken(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token != null)
        {
            token.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }
} 