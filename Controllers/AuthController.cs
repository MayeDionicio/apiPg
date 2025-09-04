using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiPG.Services;
using ApiPG.DTOs;
using System.Security.Claims;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Iniciar sesión con email y contraseña
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                
                if (result == null)
                    return Unauthorized(new { message = "Invalid email or password" });

                _logger.LogInformation("User {Email} logged in successfully", loginDto.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Email}", loginDto.Email);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Registrar un nuevo usuario
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var user = await _authService.RegisterAsync(registerDto);
                _logger.LogInformation("New user registered: {Email}", registerDto.Email);
                return CreatedAtAction(nameof(GetProfile), new { }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user {Email}", registerDto.Email);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Obtener perfil del usuario autenticado
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized(new { message = "Invalid token" });

                var user = await _authService.GetCurrentUserAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Verificar si el token JWT es válido
        /// </summary>
        [HttpGet("verify")]
        [Authorize]
        public IActionResult VerifyToken()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var usernameClaim = User.FindFirst(ClaimTypes.Name);
                var emailClaim = User.FindFirst(ClaimTypes.Email);
                var roleClaim = User.FindFirst(ClaimTypes.Role);

                return Ok(new
                {
                    message = "Token is valid",
                    user = new
                    {
                        id = userIdClaim?.Value,
                        username = usernameClaim?.Value,
                        email = emailClaim?.Value,
                        role = roleClaim?.Value
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying token");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
