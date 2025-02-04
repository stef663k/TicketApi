using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketApi.Data;
using TicketApi.DTO;
using TicketApi.Interface;
using TicketApi.Models;

namespace TicketApi.Service;

public class UserService : IUserService
{
    private readonly DatabaseContext _databaseContext;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IPasswordHasher passwordHasher, DatabaseContext databaseContext, IMapper mapper)
    {
        _passwordHasher = passwordHasher;
        _databaseContext = databaseContext;
        _mapper = mapper;
    }


    public async Task<UserResponseDTO> GetUserByIdAsync(Guid userId)
    {
        var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);
        return _mapper.Map<UserResponseDTO>(user);
    }


    public async Task<UserResponseDTO> CreateUserAsync(UserCreateDTO createDTO)
    {
        var user = _mapper.Map<User>(createDTO);
        user.PasswordHash = _passwordHasher.HashPassword(createDTO.PasswordHash);
        await _databaseContext.SaveChangesAsync();
        return _mapper.Map<UserResponseDTO>(user);
    }
    
    public async Task<UserResponseDTO?> UpdateUserAsync(Guid userId, UserUpdateDTO updateDto)
    {
        var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);

        if(user == null)
        {
            return null;
        }
        if(!string.IsNullOrEmpty(updateDto.Username))
        {
            user.Username = updateDto.Username;
        }
        if(!string.IsNullOrEmpty(updateDto.PasswordHash))
        {
            user.PasswordHash = updateDto.PasswordHash;
        }

        await _databaseContext.SaveChangesAsync();
        return _mapper.Map<UserResponseDTO>(user);
    }

    public Task<bool> DeleteUserAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserResponseDTO>> SearchUsersAsync(string searchTerm)
    {
        throw new NotImplementedException();
    }

}