using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketApi.DTO;
using TicketApi.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using TicketApi.Data;
using TicketApi.Models;
using AutoMapper.QueryableExtensions;

namespace TicketApi.Services;

public class NotificationService : INotificationService
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        DatabaseContext context, 
        IMapper mapper,
        ILogger<NotificationService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<NotificationResponseDTO> CreateNotificationAsync(
        NotificationCreateDTO createDto, 
        Guid actorUserId)
    {
        try
        {
            // Validate both users exist
            var recipientExists = await _context.Users.AnyAsync(u => u.UserId == createDto.UserId);
            var actorExists = await _context.Users.AnyAsync(u => u.UserId == actorUserId);
            
            if (!recipientExists || !actorExists)
                throw new ArgumentException("Invalid user ID(s) provided");

            var notification = _mapper.Map<Notification>(createDto);
            notification.NotificationId = Guid.NewGuid();
            notification.CreatedAt = DateTime.UtcNow;
            notification.CreatedByUserId = actorUserId;
            notification.IsRead = false;

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            return await GetNotificationWithDetails(notification.NotificationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification");
            throw;
        }
    }

    public async Task<NotificationResponseDTO?> UpdateNotificationAsync(
        Guid notificationId, 
        NotificationUpdateDTO updateDto,
        Guid actorUserId,
        bool isAdmin)
    {
        var notification = await _context.Notifications
            .Include(n => n.CreatedByUser)
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

        if (notification == null) return null;

        // Authorization check
        if (notification.CreatedByUserId != actorUserId && !isAdmin)
            throw new UnauthorizedAccessException("Only creator or admin can modify notifications");

        // Prevent changing recipient unless admin
        if (updateDto.UserId.HasValue && !isAdmin)
            throw new InvalidOperationException("Only admins can change notification recipients");

        _mapper.Map(updateDto, notification);
        await _context.SaveChangesAsync();
        
        return await GetNotificationWithDetails(notificationId);
    }

    public async Task<bool> DeleteNotificationAsync(
        Guid notificationId, 
        Guid actorUserId,
        bool isAdmin)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

        if (notification == null) return false;

        if (notification.CreatedByUserId != actorUserId && !isAdmin)
            throw new UnauthorizedAccessException("Only creator or admin can delete notifications");

        _context.Notifications.Remove(notification);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

        if (notification == null) return;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<NotificationResponseDTO?> GetNotificationByIdAsync(Guid notificationId)
    {
        return await _context.Notifications
            .Include(n => n.CreatedByUser)
            .ProjectTo<NotificationResponseDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId);
    }

    public async Task<IEnumerable<NotificationResponseDTO>> GetNotificationsByUserIdAsync(Guid userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ProjectTo<NotificationResponseDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    private async Task<NotificationResponseDTO> GetNotificationWithDetails(Guid notificationId)
    {
        return await _context.Notifications
            .Where(n => n.NotificationId == notificationId)
            .Include(n => n.CreatedByUser)
            .ProjectTo<NotificationResponseDTO>(_mapper.ConfigurationProvider)
            .FirstAsync();
    }
}