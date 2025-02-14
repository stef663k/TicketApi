using AutoMapper;
using TicketApi.DTO;
using TicketApi.Models;

namespace TicketApi;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Include all mappings from individual profiles
        CreateMapsFromAuthProfile();
        CreateMapsFromTicketProfile();
        CreateMapsFromNotificationProfile();
    }

    private void CreateMapsFromAuthProfile()
    {
        // Auth mappings
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.MapFrom(_ => UserRole.User))
            .AfterMap((src, dest) => dest.AccountCreated = DateTime.UtcNow);
            
        CreateMap<User, AuthResponse>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src));
    }

    private void CreateMapsFromTicketProfile()
    {
        // Ticket mappings
        CreateMap<Ticket, TicketDto>()
            .ForMember(dest => dest.AssignedUserId, 
                opt => opt.MapFrom(src => src.AssignedUser.UserId))
            .ForMember(dest => dest.Comments, 
                opt => opt.MapFrom(src => src.Comments));
    }

    private void CreateMapsFromNotificationProfile()
    {
        // Notification mappings
        CreateMap<Notification, NotificationResponseDTO>()
            .ForMember(dest => dest.User, 
                opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.CreatedByUser, 
                opt => opt.MapFrom(src => src.CreatedByUser));
    }
} 