using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiPG.Services;
using ApiPG.DTOs;
using System.Security.Claims;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/perfiles")]
    [Authorize]
    public class PerfilesController : ControllerBase
    {
        private readonly IServicioDePerfil _perfilService;

        public PerfilesController(IServicioDePerfil perfilService)
        {
            _perfilService = perfilService;
        }

        private int ObtenerUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        /// <summary>
        /// Obtener mi perfil (usuario autenticado)
        /// </summary>
        [HttpGet("mi-perfil")]
        public async Task<ActionResult<PerfilUsuarioDto>> ObtenerMiPerfil()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var perfil = await _perfilService.ObtenerPerfilPorUsuarioIdAsync(usuarioId);

                if (perfil == null)
                    return NotFound(new { message = "Perfil no encontrado" });

                return Ok(perfil);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener perfil", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener perfil de un usuario por ID (solo coordinadores)
        /// </summary>
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<PerfilUsuarioDto>> ObtenerPerfilPorUsuarioId(int usuarioId)
        {
            try
            {
                var perfil = await _perfilService.ObtenerPerfilPorUsuarioIdAsync(usuarioId);

                if (perfil == null)
                    return NotFound(new { message = "Perfil no encontrado" });

                return Ok(perfil);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener perfil", error = ex.Message });
            }
        }

        /// <summary>
        /// Crear mi perfil
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PerfilUsuarioDto>> CrearPerfil([FromBody] CrearPerfilDto dto)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var perfil = await _perfilService.CrearPerfilAsync(usuarioId, dto);
                return CreatedAtAction(nameof(ObtenerMiPerfil), perfil);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear perfil", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar mi perfil
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<PerfilUsuarioDto>> ActualizarPerfil([FromBody] ActualizarPerfilDto dto)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var perfil = await _perfilService.ActualizarPerfilAsync(usuarioId, dto);

                if (perfil == null)
                    return NotFound(new { message = "Perfil no encontrado" });

                return Ok(perfil);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar perfil", error = ex.Message });
            }
        }

        /// <summary>
        /// Subir foto de perfil
        /// </summary>
        [HttpPost("foto")]
        public async Task<ActionResult> SubirFotoPerfil([FromBody] SubirFotoPerfilDto dto)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var urlFoto = await _perfilService.GuardarFotoPerfilAsync(usuarioId, dto);
                
                return Ok(new { 
                    message = "Foto subida exitosamente", 
                    urlFoto = urlFoto 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al subir foto", error = ex.Message });
            }
        }

        /// <summary>
        /// Eliminar foto de perfil
        /// </summary>
        [HttpDelete("foto")]
        public async Task<IActionResult> EliminarFotoPerfil()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var eliminado = await _perfilService.EliminarFotoPerfilAsync(usuarioId);

                if (!eliminado)
                    return NotFound(new { message = "No hay foto para eliminar" });

                return Ok(new { message = "Foto eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar foto", error = ex.Message });
            }
        }
    }
}
