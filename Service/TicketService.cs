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

public class TicketService : ITicketService
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public TicketService(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TicketDto> CreateTicketAsync(CreateTicketDto dto, Guid userId)
    {
        var ticket = _mapper.Map<Ticket>(dto);
        ticket.UserId = userId;
        ticket.CreatedAt = DateTime.UtcNow;
        ticket.Status = TicketStatus.New;
        ticket.ClosedTime = null; 
        
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<TicketDto>(ticket);
    }

    public async Task<TicketDto?> GetTicketByIdAsync(Guid ticketId)
    {
        var ticket = await _context.Tickets
            .Include(t => t.User)
            .Include(t => t.AssignedUser)
            .Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.TicketId == ticketId);
            
        return ticket == null ? null : _mapper.Map<TicketDto>(ticket);
    }

    public async Task<IEnumerable<TicketDto>> GetTicketsByUserAsync(Guid userId)
    {
        var tickets = await _context.Tickets
            .Include(t => t.AssignedUser)
            .Include(t => t.Comments)
            .Where(t => t.UserId == userId || t.AssignedUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }

    public async Task<IEnumerable<TicketDto>> SearchTicketsAsync(string searchTerm)
    {
        var tickets = await _context.Tickets
            .Include(t => t.User)
            .Include(t => t.Comments)
            .Where(t => EF.Functions.Like(t.Title, $"%{searchTerm}%") || 
                       EF.Functions.Like(t.Description, $"%{searchTerm}%"))
            .Take(100)
            .ToListAsync();

        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }

    public async Task<TicketDto?> UpdateTicketAsync(Guid ticketId, UpdateTicketDto dto)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Title))
            ticket.Title = dto.Title;
            
        if (!string.IsNullOrWhiteSpace(dto.Description))
            ticket.Description = dto.Description;
            
        if (dto.Category.HasValue)
            ticket.Category = dto.Category.Value;
            
        if (dto.Priority.HasValue)
            ticket.Priority = dto.Priority.Value;

        await _context.SaveChangesAsync();
        return _mapper.Map<TicketDto>(ticket);
    }

    public async Task<bool> DeleteTicketAsync(Guid ticketId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null) return false;

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TicketDto?> UpdateTicketStatusAsync(Guid ticketId, TicketStatus status)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null) return null;

        // Handle closed time logic
        if (status == TicketStatus.Closed && ticket.Status != TicketStatus.Closed)
        {
            ticket.ClosedTime = DateTime.UtcNow;
        }
        else if (status != TicketStatus.Closed && ticket.Status == TicketStatus.Closed)
        {
            ticket.ClosedTime = null;
        }
        
        ticket.Status = status;
        await _context.SaveChangesAsync();
        return _mapper.Map<TicketDto>(ticket);
    }

    public async Task<TicketDto?> AssignTicketAsync(Guid ticketId, Guid? assigneeId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null) return null;

        if (assigneeId.HasValue)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == assigneeId.Value);
            if (!userExists) return null;
        }

        ticket.AssignedUserId = assigneeId;
        await _context.SaveChangesAsync();
        return _mapper.Map<TicketDto>(ticket);
    }

    public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
    {
        var tickets = await _context.Tickets
            .Include(t => t.User)
            .Include(t => t.AssignedUser)
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }
}