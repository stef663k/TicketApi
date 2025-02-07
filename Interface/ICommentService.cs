using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketApi.DTO;
using TicketApi.Models;

namespace TicketApi.Interface
{
    public interface ICommentService
    {
        Task<CommentDto> CreateCommentAsync(CreateCommentDto dto, Guid userId);
        Task<CommentDto?> GetCommentByIdAsync(Guid commentId);
        Task<IEnumerable<CommentDto>> GetCommentsByTicketAsync(Guid ticketId);
        Task<CommentDto?> UpdateCommentAsync(Guid commentId, UpdateCommentDto dto, Guid userId);
        Task<bool> DeleteCommentAsync(Guid commentId);
    }
}