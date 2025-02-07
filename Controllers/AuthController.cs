using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketApi.DTO;
using TicketApi.Service;

namespace TicketApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            return result == null ? Unauthorized() : Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDTO>> Register(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            return result == null ? BadRequest("Username already exists") : Ok(result);
        }
    }
}
