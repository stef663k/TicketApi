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


public class AuthService : IAuthService
{
    private readonly DatabaseContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(
        DatabaseContext context,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IMapper mapper)
    {
        _context = context;
        _passwordHasher = passwordHasher;
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
            
            Console.WriteLine($"Password match: {_passwordHasher.VerifyPassword(user.PasswordHash, loginDto.Password)}");
            
            Console.WriteLine($"[{DateTime.UtcNow}] Login attempt for: {loginDto.Username}");
            
            Console.WriteLine($"User found: {user.UserId} | Role: {user.Role} | Last Login: {user.LastLogin}");
            Console.WriteLine($"Stored hash: {user.PasswordHash}");
            
            if (!_passwordHasher.VerifyPassword(user.PasswordHash, loginDto.Password))
            {
                Console.WriteLine("Password verification failed");
                return null;
            }

            Console.WriteLine("Password verified");
            var token = _tokenService.GenerateToken(user);
            Console.WriteLine($"Token generated: {token?.Substring(0, 20)}... (Full length: {token?.Length})");
            
            var response = _mapper.Map<AuthResponse>(user);
            response!.AccessToken = token ?? throw new InvalidOperationException("Token generation failed");
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {DateTime.UtcNow}: {ex}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw; // Re-throw to preserve original error
        }
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterDto registerDto)
    {
        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = _passwordHasher.HashPassword(registerDto.Password),
            Role = registerDto.Role,
        };
        
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