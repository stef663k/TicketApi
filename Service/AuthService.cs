using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketApi.Data;
using TicketApi.Models;
using TicketApi.Service;
using TicketApi.DTO;
using TicketApi.Interface;
using AutoMapper;


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
    }

    public async Task<AuthResponse?> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username && !u.IsDeleted);

        if (user == null || !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            return null;

        return new AuthResponse
        {
            Token = _tokenService.GenerateToken(user),
            Expiration = DateTime.UtcNow.AddHours(2),
            User = _mapper.Map<UserResponseDTO>(user)
        };
    }

    public async Task<UserResponseDTO?> RegisterAsync(RegisterDto registerDto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            return null;

        var user = _mapper.Map<User>(registerDto);
        user.PasswordHash = _passwordHasher.HashPassword(registerDto.Password);
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<UserResponseDTO>(user);
    }
} 