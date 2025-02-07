
using TicketApi.Models;

public interface ITokenService
{
    string GenerateToken(User user);
}