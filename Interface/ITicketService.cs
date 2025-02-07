using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketApi.DTO;
using TicketApi.Models;

namespace TicketApi.Interface;

public interface ITicketService
{
    Task<TicketDto> CreateTicketAsync(CreateTicketDto dto, Guid userId);
    Task<TicketDto?> GetTicketByIdAsync(Guid ticketId);
    Task<IEnumerable<TicketDto>> GetTicketsByUserAsync(Guid userId);
    Task<IEnumerable<TicketDto>> SearchTicketsAsync(string searchTerm);
    Task<TicketDto?> UpdateTicketAsync(Guid ticketId, UpdateTicketDto dto);
    Task<bool> DeleteTicketAsync(Guid ticketId);
    Task<TicketDto?> UpdateTicketStatusAsync(Guid ticketId, TicketStatus status);
    Task<TicketDto?> AssignTicketAsync(Guid ticketId, Guid? assigneeId);
}
