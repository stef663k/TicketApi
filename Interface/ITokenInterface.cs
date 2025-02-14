
using TicketApi.Models;

public interface ITokenService
{
    string GenerateToken(User user);
    public string GenerateAccessToken(User user);
    public string GenerateRefreshToken();

}