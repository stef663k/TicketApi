using System.Security.Claims;
using Microsoft.Extensions.Logging;
using TicketApi.Models;

namespace TicketApi.Extensions;

public static class UserExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier); // Use FindFirstValue instead of FindFirst

        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User ID claim not found in the token.");
        }

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        throw new FormatException("Invalid UserId format in the token.");
    }



    public static bool IsAdminOrSupporter(this ClaimsPrincipal user)
    {
        return user.IsInRole("Admin") || user.IsInRole("Supporter");
    }
}
