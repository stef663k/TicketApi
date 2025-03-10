using AutoMapper;
using TicketApi.DTO;
using TicketApi.Models;

namespace TicketApi.Profiles;

public class AuthMappings : Profile
{
    public AuthMappings()
    {
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.AccountCreated, opt => opt.Ignore())
            .ForMember(dest => dest.LastLogin, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

        CreateMap<User, UserResponseDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.AccountCreated, opt => opt.MapFrom(src => src.AccountCreated))
            .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLogin));

        CreateMap<User, AuthResponse>()
            .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src));
    }
} 