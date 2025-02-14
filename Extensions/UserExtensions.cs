using System.Security.Claims;
namespace TicketApi.Extensions;

public static class UserExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User ID claim not found");

        return Guid.Parse(userIdClaim);
    }
        public static bool IsAdminOrSupporter(this ClaimsPrincipal user)
    {
        return user.IsInRole("Admin") || user.IsInRole("Supporter");
    }

} 