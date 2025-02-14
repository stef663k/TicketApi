using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketApi.Data;
using TicketApi.Models;
using TicketApi.DTO;
using TicketApi.Interface;
using TicketApi.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TicketApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET all users (Admin only)
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        // GET specific user (Admin/Supporter can access any, regular users only themselves)
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserResponseDTO>> GetUser(Guid userId)
        {
            var currentUserId = User.GetUserId();
            var isAdminOrSupporter = User.IsAdminOrSupporter();
            
            if (userId != currentUserId && !isAdminOrSupporter)
                return Forbid();

            var user = await _userService.GetUserByIdAsync(userId);
            return user != null ? Ok(user) : NotFound();
        }

        // PUT update user (Admin/Supporter can update any, regular users only themselves)
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserUpdateDTO updateDto)
        {
            var currentUserId = User.GetUserId();
            var isAdminOrSupporter = User.IsAdminOrSupporter();
            
            if (userId != currentUserId && !isAdminOrSupporter)
                return Forbid();

            var updatedUser = await _userService.UpdateUserAsync(userId, updateDto);
            return updatedUser != null ? Ok(updatedUser) : NotFound();
        }

        // POST create user (open registration)
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserResponseDTO>> CreateUser([FromBody] UserCreateDTO createDto)
        {
            var createdUser = await _userService.CreateUserAsync(createDto);
            return CreatedAtAction(nameof(GetUser), new { userId = createdUser.UserId }, createdUser);
        }

        // DELETE user (Admin only)
        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var success = await _userService.DeleteUserAsync(userId);
            return success ? NoContent() : NotFound();
        }

        // Search users (Admin/Supporter only)
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Supporter")]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> SearchUsers([FromQuery] string term)
        {
            var results = await _userService.SearchUsersAsync(term);
            return Ok(results);
        }
    }
}
