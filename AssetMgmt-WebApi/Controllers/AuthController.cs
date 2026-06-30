using AssetMgmt_WebApi.DTOs;
using AssetMgmt_WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetMgmt_WebApi.Controllers
{
    //User Registration and login
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // POST api/auth/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var (success, message) = await _userService.RegisterAsync(dto);
                if (!success) return Conflict(new { message });

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering user {Username}", dto.Username);
                return StatusCode(500, new { message = "An unexpected error occurred during registration." });
            }
        }

        // POST api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _userService.AuthenticateAsync(dto);
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            return Ok(result);
        }
    }
}
