using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using TicketApi.Models;

namespace TicketApi.DTO;

public class TicketDto
{
    public Guid TicketId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketCategory Category { get; set; }
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedTime { get; set; }
    public Guid? AssignedUserId { get; set; }
    public Guid UserId { get; set; }
    public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
}

public class CreateTicketDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public TicketCategory Category { get; set; }
    
    [Required]
    public TicketPriority Priority { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
}

public class UpdateTicketDto
{
    [StringLength(100)]
    public string? Title { get; set; }
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    public TicketCategory? Category { get; set; }
    public TicketPriority? Priority { get; set; }
}

public class UpdateTicketStatusDto
{
    [Required]
    public TicketStatus Status { get; set; }
}

public class AssignTicketDto
{
    public Guid? AssignedUserId { get; set; }
}

// AutoMapper Profile
public class TicketMappingProfile : Profile
{
    public TicketMappingProfile()
    {
        // Ticket Mappings
        CreateMap<Ticket, TicketDto>()
            .ForMember(dest => dest.AssignedUserId, 
                opt => opt.MapFrom(src => src.AssignedUser))
            .ForMember(dest => dest.AssignedUserId, 
                opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Comments, 
                opt => opt.MapFrom(src => src.Comments));

        CreateMap<CreateTicketDto, Ticket>()
            .ForMember(dest => dest.CreatedAt, 
                opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Status, 
                opt => opt.MapFrom(_ => TicketStatus.New));

        CreateMap<UpdateTicketDto, Ticket>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
