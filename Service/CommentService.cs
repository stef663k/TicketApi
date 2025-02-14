using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketApi.Data;
using TicketApi.DTO;
using TicketApi.Interface;
using TicketApi.Models;

namespace TicketApi.Service;

public class CommentService : ICommentService
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public CommentService(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CommentDto> CreateCommentAsync(CreateCommentDto dto, Guid userId)
    {
        var comment = _mapper.Map<Comment>(dto);
        comment.UserId = userId;
        comment.CreatedAt = DateTime.UtcNow;
        
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<CommentDto>(comment);
    }

    public async Task<CommentDto?> GetCommentByIdAsync(Guid commentId)
    {
        var comment = await _context.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.CommentId == commentId && !c.IsDeleted);
            
        return comment == null ? null : _mapper.Map<CommentDto>(comment);
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByTicketAsync(Guid ticketId)
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.TicketId == ticketId && !c.IsDeleted)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task<CommentDto?> UpdateCommentAsync(
        Guid commentId, 
        UpdateCommentDto dto,
        Guid userId)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == userId);
        
        if (comment == null) return null;
        
        _mapper.Map(dto, comment);
        await _context.SaveChangesAsync();
        return _mapper.Map<CommentDto>(comment);
    }

    public async Task<bool> DeleteCommentAsync(
        Guid commentId, 
        Guid userId,
        bool isAdminOrSupporter)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null) return false;

        if (comment.UserId != userId && !isAdminOrSupporter)
            return false;

        _context.Comments.Remove(comment);
        return await _context.SaveChangesAsync() > 0;
    }

}