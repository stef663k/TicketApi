using System;
using System.ComponentModel.DataAnnotations;
using TicketApi.Models;

namespace TicketApi.DTO;

public class TicketDto
{
    public Guid TicketId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketCategory Category { get; set; }
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedTime { get; set; }
    public Guid? AssignedUserId { get; set; }
    public Guid UserId { get; set; }
}

public class CreateTicketDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public TicketCategory Category { get; set; }
    
    [Required]
    public TicketPriority Priority { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
}

public class UpdateTicketDto
{
    [StringLength(100)]
    public string? Title { get; set; }
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    public TicketCategory? Category { get; set; }
    public TicketPriority? Priority { get; set; }
}

public class UpdateTicketStatusDto
{
    [Required]
    public TicketStatus Status { get; set; }
}

public class AssignTicketDto
{
    public Guid? AssignedUserId { get; set; }
}
