using System.Security.Claims;
using TicketApi.Models;

namespace TicketApi.Interface;
public interface ITokenService
{
    string GenerateToken(User user);
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    bool ValidateRefreshToken(string refreshToken, Guid userId);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}