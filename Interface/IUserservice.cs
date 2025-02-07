using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketApi.DTO;

namespace TicketApi.Interface;

public interface IUserService
{
    Task<UserResponseDTO?> GetUserByIdAsync(Guid userId);
    Task<UserResponseDTO> CreateUserAsync(UserCreateDTO createDto);
    Task<UserResponseDTO?> UpdateUserAsync(Guid userId, UserUpdateDTO updateDto);
    Task<bool> DeleteUserAsync(Guid userId);
    Task<IEnumerable<UserResponseDTO>> SearchUsersAsync(string searchTerm);
    Task<IEnumerable<UserResponseDTO>> GetUsersAsync();
}