using TicketApi.DTO;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginDto loginDto);
    Task<AuthResponse?> RegisterAsync(RegisterDto registerDto);
    public Task RevokeRefreshToken(string refreshToken);

}