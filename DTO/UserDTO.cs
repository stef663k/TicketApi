using System;
using System.ComponentModel.DataAnnotations;
using TicketApi.Models;
using AutoMapper;

namespace TicketApi.DTO;

public class UserCreateDTO
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }
}

public class UserUpdateDTO
{
    [StringLength(50, MinimumLength = 3)]
    public string? Username { get; set; }
    
    [StringLength(100, MinimumLength = 8)]
    public string? PasswordHash { get; set; }
} 

public class UserResponseDTO
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string Role { get; set; }
    public DateTime AccountCreated { get; set; }
    public DateTime? LastLogin { get; set; }
}

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserResponseDTO>();
        CreateMap<UserCreateDTO, User>();
        CreateMap<UserUpdateDTO, User>();
    }
}