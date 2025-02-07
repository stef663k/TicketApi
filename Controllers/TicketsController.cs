using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketApi.Data;
using TicketApi.DTO;
using TicketApi.Interface;
using TicketApi.Models;
using TicketApi.Service;
using Microsoft.AspNetCore.Authorization;

namespace TicketApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: api/Tickets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        // GET: api/Tickets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> GetTicket(Guid id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            return ticket == null ? NotFound() : Ok(ticket);
        }

        // PUT: api/Tickets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicket(Guid id, UpdateTicketDto updateDto)
        {
            var result = await _ticketService.UpdateTicketAsync(id, updateDto);
            return result == null ? NotFound() : NoContent();
        }

        // POST: api/Tickets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TicketDto>> PostTicket(CreateTicketDto createDto)
        {
            var userId = User.GetUserId();
            var ticket = await _ticketService.CreateTicketAsync(createDto, userId);
            return CreatedAtAction("GetTicket", new { id = ticket.TicketId }, ticket);
        }

        // DELETE: api/Tickets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            var result = await _ticketService.DeleteTicketAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
