using System;
using System.ComponentModel.DataAnnotations;
using TicketApi.Models;


namespace TicketApi.Models;

public class Comment
{
    public Guid CommentId { get; set; } = Guid.NewGuid();
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid TicketId { get; set; }
    public required Ticket Ticket { get; set; }


    public Guid UserId { get; set; }
    public required User User { get; set; }
}