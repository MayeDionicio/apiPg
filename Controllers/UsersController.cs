using Microsoft.AspNetCore.Mvc;
using ApiPG.Services;
using ApiPG.DTOs;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Obtiene todos los usuarios activos
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>El usuario encontrado</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            
            if (user == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado");
            }

            return Ok(user);
        }

        /// <summary>
        /// Obtiene un usuario por su nombre de usuario
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <returns>El usuario encontrado</returns>
        [HttpGet("by-username/{username}")]
        public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            
            if (user == null)
            {
                return NotFound($"Usuario con username '{username}' no encontrado");
            }

            return Ok(user);
        }

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        /// <param name="createUserDto">Datos del usuario a crear</param>
        /// <returns>El usuario creado</returns>
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        /// <param name="id">ID del usuario a actualizar</param>
        /// <param name="updateUserDto">Datos del usuario a actualizar</param>
        /// <returns>El usuario actualizado</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                
                if (user == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado");
                }

                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Elimina un usuario (soft delete)
        /// </summary>
        /// <param name="id">ID del usuario a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            
            if (!deleted)
            {
                return NotFound($"Usuario con ID {id} no encontrado");
            }

            return NoContent();
        }

        /// <summary>
        /// Verifica si un usuario existe por username o email
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="email">Email</param>
        /// <returns>Resultado de la verificación</returns>
        [HttpGet("exists")]
        public async Task<ActionResult<object>> CheckUserExists([FromQuery] string username, [FromQuery] string email)
        {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
            {
                return BadRequest("Debe proporcionar username o email");
            }

            var exists = await _userService.UserExistsAsync(username ?? "", email ?? "");
            return Ok(new { exists, username, email });
        }
    }
}
