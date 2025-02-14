using AutoMapper;
using TicketApi.DTO;
using TicketApi.Models;

namespace TicketApi.Profiles;

public class AuthMappings : Profile
{
    public AuthMappings()
    {
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .AfterMap((src, dest) => {
                dest.AccountCreated = DateTime.UtcNow;
                dest.IsDeleted = false;
                dest.LastLogin = null;
            });

        CreateMap<User, UserResponseDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.AccountCreated, opt => opt.MapFrom(src => src.AccountCreated));

        CreateMap<User, AuthResponse>()
            .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src));
    }
} 