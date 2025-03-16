using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketApi.DTO;
using TicketApi.Extensions;
using TicketApi.Interface;

namespace TicketApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var validUserId))
        {
            throw new UnauthorizedAccessException("Invalid user identity");
        }
        return validUserId;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,Supporter")]
    public async Task<IActionResult> CreateNotification([FromBody] NotificationCreateDTO dto)
    {
        try
        {
            var actorUserId = GetCurrentUserId();
            var result = await _notificationService.CreateNotificationAsync(dto, actorUserId);
            return CreatedAtAction(nameof(GetNotification), new { id = result.NotificationId }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid notification creation attempt");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotification(Guid id)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(id);
        var currentUserId = User.GetUserId();
        
        // Only allow access if user is recipient, creator, or admin/supporter
        if (notification?.UserId != currentUserId && 
            notification?.CreatedByUserId != currentUserId && 
            !User.IsAdminOrSupporter())
        {
            return Forbid();
        }
        
        return notification != null ? Ok(notification) : NotFound();
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserNotifications()
    {
        var userId = User.GetUserId();
        var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
        return Ok(notifications);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator,Supporter")]
    public async Task<IActionResult> UpdateNotification(
        Guid id, 
        [FromBody] NotificationUpdateDTO dto)
    {
        try
        {
            var actorUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");
            
            var result = await _notificationService.UpdateNotificationAsync(
                id, dto, actorUserId, isAdmin);
            
            return result != null ? Ok(result) : NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized notification update attempt");
            return Forbid(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator,Supporter")]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        try
        {
            var actorUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");
            
            var success = await _notificationService.DeleteNotificationAsync(id, actorUserId, isAdmin);
            return success ? NoContent() : NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized notification deletion attempt");
            return Forbid(ex.Message);
        }
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(id);
        if (notification?.UserId != User.GetUserId())
        {
            return Forbid("Cannot mark another user's notification as read");
        }
        
        await _notificationService.MarkAsReadAsync(id);
        return NoContent();
    }
}