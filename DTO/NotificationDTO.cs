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

    [Required(ErrorMessage = "Recipient User ID is required")]
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
    public string Content { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public UserResponseDTO? User { get; set; }          // Recipient info
    public Guid CreatedByUserId { get; set; }
    public UserResponseDTO? CreatedByUser { get; set; }  // Creator info
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<Notification, NotificationResponseDTO>()
            .ForMember(dest => dest.User, 
                opt => opt.MapFrom(src => src.User)) // Maps recipient
            .ForMember(dest => dest.CreatedByUser, 
                opt => opt.MapFrom(src => src.CreatedByUser)); // Maps creator
    }
}