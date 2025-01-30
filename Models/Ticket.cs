using System;
using System.Collections.Generic;
using TicketApi.Models;


namespace TicketApi.Models;

public class Ticket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketCategory Category { get; set; }
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? AssignedUserId { get; set; }
    public required User AssignedUser { get; set; }

    public Guid UserId { get; set; }
    public required User User { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public enum TicketCategory
{
    Hardware,
    Software,
    Network,
    Other
}

public enum TicketPriority
{
    Low,
    Medium,
    High
}

public enum TicketStatus
{
    New,
    InProgress,
    AwaitingUser,
    Closed
}