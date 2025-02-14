using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketApi.DTO;
using TicketApi.Models;

namespace TicketApi.Interface;

public interface INotificationService
{
    Task<NotificationResponseDTO?> GetNotificationByIdAsync(Guid notificationId);
    Task<IEnumerable<NotificationResponseDTO>> GetNotificationsByUserIdAsync(Guid userId);
    Task<NotificationResponseDTO> CreateNotificationAsync(NotificationCreateDTO notificationDto, Guid actorUserId);
    Task<NotificationResponseDTO?> UpdateNotificationAsync(Guid notificationId, NotificationUpdateDTO notificationDto, Guid actorUserId, bool isAdmin);
    Task<bool> DeleteNotificationAsync(Guid notificationId, Guid actorUserId, bool isAdmin);
    Task MarkAsReadAsync(Guid notificationId);
}