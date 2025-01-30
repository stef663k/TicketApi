using System;
using System.ComponentModel.DataAnnotations;
using TicketApi.Models;

namespace TicketApi.Models;

public class Notification
{
    public Guid NotificationId { get; set; } = Guid.NewGuid();
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public required User User { get; set; }
    public Guid UserId { get; set; }

}

public enum NotificationType
{
    TicketCreated,
    TicketUpdated,
    TicketClosed,
    CommentAdded,
    AssignedToUser
}