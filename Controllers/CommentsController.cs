using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketApi.DTO;
using TicketApi.Extensions;
using TicketApi.Interface;

namespace TicketApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("{ticketId}")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetByTicket(Guid ticketId)
    {
        var comments = await _commentService.GetCommentsByTicketAsync(ticketId);
        return Ok(comments);
    }

    [HttpGet("single/{commentId}")]
    public async Task<ActionResult<CommentDto>> GetById(Guid commentId)
    {
        var comment = await _commentService.GetCommentByIdAsync(commentId);
        return comment == null ? NotFound() : Ok(comment);
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> Create([FromBody] CreateCommentDto dto)
    {
        // Parse the user ID from the 'sub' claim
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new InvalidOperationException("User ID claim is invalid or not found.");
        }

        // Create the comment
        var comment = await _commentService.CreateCommentAsync(dto, userId);

        // Return the created comment
        return CreatedAtAction(nameof(GetById), new { commentId = comment.CommentId }, comment);
    }

    [HttpPut("{commentId}")]
    public async Task<IActionResult> Update(Guid commentId, [FromBody] UpdateCommentDto dto)
    {
        // Parse the user ID from the 'sub' claim
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new InvalidOperationException("User ID claim is invalid or not found.");
        }

        // Update the comment
        var result = await _commentService.UpdateCommentAsync(commentId, dto, userId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> Delete(Guid commentId)
    {
        // Retrieve user ID and check if the user is admin or supporter
        var userId = User.GetUserId();
        var isAdminOrSupporter = User.IsAdminOrSupporter();

        // Attempt to delete the comment
        var success = await _commentService.DeleteCommentAsync(
            commentId, 
            userId, 
            isAdminOrSupporter);
        
        // Return appropriate response based on the success of the delete operation
        return success ? NoContent() : NotFound();
    }
}
