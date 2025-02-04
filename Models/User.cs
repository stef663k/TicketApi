using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicketApi.Models;

namespace TicketApi.Models;
public class User
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    [Required]
    public DateTime AccountCreated { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();
}

public enum UserRole
{
    Administrator,
    Support,
    User
}