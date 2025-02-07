using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketApi.Models;


namespace TicketApi.Models;

public class Comment
{
    [Key]
    public Guid CommentId { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(1000, ErrorMessage = "Comment text cannot exceed 1000 characters")]
    public string Text { get; set; } = string.Empty;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted {get; set;} = false;

    [Required]
    public Guid TicketId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public required Ticket Ticket { get; set; }
    public required User User { get; set; }
}