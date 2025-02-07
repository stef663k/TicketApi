using TicketApi.DTO;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginDto loginDto);
    Task<UserResponseDTO?> RegisterAsync(RegisterDto registerDto);
}