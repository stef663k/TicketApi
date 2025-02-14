using AutoMapper;
using TicketApi.DTO;
using TicketApi.Models;

namespace TicketApi.Profiles;

public class TicketMappings : Profile
{
    public TicketMappings()
    {
        CreateMap<Ticket, TicketDto>()
            .ForMember(dest => dest.AssignedUserId, 
                opt => opt.MapFrom(src => src.AssignedUser != null ? src.AssignedUser.UserId : (Guid?)null))
            .ForMember(dest => dest.Comments, 
                opt => opt.MapFrom(src => src.Comments));

        CreateMap<CreateTicketDto, Ticket>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => TicketStatus.New));

        CreateMap<UpdateTicketDto, Ticket>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
} 