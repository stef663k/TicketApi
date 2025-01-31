using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using TicketApi.Models;

namespace TicketApi.DTO;

public class NotificationDTO
{
    
}

public class NotificationCreateDTO
{
    [Required(ErrorMessage = "Message is required")]
    [StringLength(500, ErrorMessage = "Message must be under 500 characters")]
    public string Message { get; set; } = string.Empty;

    [Required(ErrorMessage = "Notification type is required")]
    public NotificationType Type { get; set; }

    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId { get; set; }
}

public class NotificationUpdateDTO
{
    [StringLength(500, ErrorMessage = "Message must be under 500 characters")]
    public string? Message { get; set; }
    
    public NotificationType? Type { get; set; }
    public Guid? UserId { get; set; }
}

public class NotificationResponseDTO
{
    public Guid NotificationId { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
}

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<NotificationCreateDTO, Notification>()
            .ForMember(dest => dest.CreatedAt, 
                       opt => opt.MapFrom(_ => DateTime.UtcNow));
            
        CreateMap<NotificationUpdateDTO, Notification>()
            .ForAllMembers(opts => opts.Condition(
                (src, dest, srcMember) => srcMember != null));
            
        CreateMap<Notification, NotificationResponseDTO>();
    }
}