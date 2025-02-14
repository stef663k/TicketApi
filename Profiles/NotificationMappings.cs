using AutoMapper;
using TicketApi.DTO;
using TicketApi.Models;

namespace TicketApi.Profiles;

public class NotificationMappings : Profile
{
    public NotificationMappings()
    {
        CreateMap<Notification, NotificationResponseDTO>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.CreatedByUser, opt => opt.MapFrom(src => src.CreatedByUser));
    }
} 