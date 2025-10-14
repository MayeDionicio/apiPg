using Microsoft.AspNetCore.Mvc;
using ApiPG.Services;
using ApiPG.DTOs;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Obtiene todos los roles activos
        /// </summary>
        /// <returns>Lista de roles</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolDto>>> GetRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Obtiene un rol por su ID
        /// </summary>
        /// <param name="id">ID del rol</param>
        /// <returns>El rol encontrado</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<RolDto>> GetRole(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            
            if (role == null)
            {
                return NotFound($"Rol con ID {id} no encontrado");
            }

            return Ok(role);
        }

        /// <summary>
        /// Crea un nuevo rol
        /// </summary>
        /// <param name="createRoleDto">Datos del rol a crear</param>
        /// <returns>El rol creado</returns>
        [HttpPost]
        public async Task<ActionResult<RolDto>> CreateRole(CrearRolDto createRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var role = await _roleService.CreateRoleAsync(createRoleDto);
                return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza un rol existente
        /// </summary>
        /// <param name="id">ID del rol a actualizar</param>
        /// <param name="updateRoleDto">Datos del rol a actualizar</param>
        /// <returns>El rol actualizado</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<RolDto>> UpdateRole(int id, ActualizarRolDto updateRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var role = await _roleService.UpdateRoleAsync(id, updateRoleDto);
                
                if (role == null)
                {
                    return NotFound($"Rol con ID {id} no encontrado");
                }

                return Ok(role);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Elimina un rol (soft delete)
        /// </summary>
        /// <param name="id">ID del rol a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var deleted = await _roleService.DeleteRoleAsync(id);
                
                if (!deleted)
                {
                    return NotFound($"Rol con ID {id} no encontrado");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Verifica si un rol existe por nombre
        /// </summary>
        /// <param name="name">Nombre del rol</param>
        /// <returns>Resultado de la verificación</returns>
        [HttpGet("existe")]
        public async Task<ActionResult<object>> CheckRoleExists([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Debe proporcionar el nombre del rol");
            }

            var exists = await _roleService.RoleExistsAsync(name);
            return Ok(new { exists, name });
        }

        /// <summary>
        /// Verifica si un rol tiene usuarios asignados
        /// </summary>
        /// <param name="id">ID del rol</param>
        /// <returns>Resultado de la verificación</returns>
        [HttpGet("{id}/tiene-usuarios")]
        public async Task<ActionResult<object>> CheckRoleHasUsers(int id)
        {
            var hasUsers = await _roleService.RoleHasUsersAsync(id);
            return Ok(new { roleId = id, hasUsers });
        }
    }
}
