using System;
using System.ComponentModel.DataAnnotations;
using TicketApi.Models;

namespace TicketApi.DTO;

public class CommentDto
{
    public Guid CommentId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
}

public class CreateCommentDto
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
    
    [Required]
    public Guid TicketId { get; set; }
}

public class UpdateCommentDto
{
    [StringLength(1000, MinimumLength = 1)]
    public string? Text { get; set; }
}
