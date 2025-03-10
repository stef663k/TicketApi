using System.ComponentModel.DataAnnotations;
using TicketApi.DTO;
using AutoMapper;
using TicketApi.Models;
using System.Data;

public class LoginDto
{
    [Required]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class RegisterDto
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    [Required]
    public UserRole Role{ get; set; } 
}

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public UserResponseDTO User { get; set; } = null!;
}

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
         CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.Username, 
                opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, 
                opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PasswordHash, 
                opt => opt.Ignore())
            .ForMember(dest => dest.UserId, 
                opt => opt.Ignore())
            .ForMember(dest => dest.Role, 
                opt => opt.MapFrom(_ => UserRole.User))
            .ForMember(dest => dest.AccountCreated, 
                opt => opt.MapFrom(_ => DateTime.UtcNow))
            .AfterMap((src, dest) => 
            {
                dest.IsDeleted = false;
                dest.LastLogin = null;
                dest.AssignedTickets = new List<Ticket>();
                dest.Comments = new List<Comment>();
                dest.Notifications = new List<Notification>();
                dest.CreatedTickets = new List<Ticket>();
            });

        CreateMap<User, AuthResponse>()
            .ForMember(dest => dest.User, 
                opt => opt.MapFrom(src => src));
    }
}
