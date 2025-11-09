using ApiPG.DTOs;
using ApiPG.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ControladorDeLogrosFacilitador : ControllerBase
    {
        private readonly IServicioDeLogrosFacilitador _servicio;
        private readonly ILogger<ControladorDeLogrosFacilitador> _logger;

        public ControladorDeLogrosFacilitador(
            IServicioDeLogrosFacilitador servicio,
            ILogger<ControladorDeLogrosFacilitador> logger)
        {
            _servicio = servicio;
            _logger = logger;
        }

        /// <summary>
        /// Crea un nuevo logro (Solo Coordinadores)
        /// </summary>
        [HttpPost("logros")]
        public async Task<ActionResult<LogroFacilitadorDto>> CrearLogro([FromBody] CrearLogroFacilitadorDto dto)
        {
            try
            {
                // Verificar que es coordinador (IdRol = 5)
                var roleClaim = User.FindFirst("RoleId")?.Value;
                if (string.IsNullOrEmpty(roleClaim) || roleClaim != "5")
                {
                    return StatusCode(403, new { mensaje = "Solo los coordinadores pueden crear logros" });
                }

                var usuarioIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(usuarioIdClaim) || !int.TryParse(usuarioIdClaim, out int usuarioId))
                {
                    return StatusCode(403, new { mensaje = "No se pudo identificar al usuario" });
                }

                var resultado = await _servicio.CrearLogroAsync(dto, usuarioId);
                return CreatedAtAction(nameof(ObtenerLogroPorId), new { id = resultado.Id }, resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear logro: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un logro por ID
        /// </summary>
        [HttpGet("logros/{id}")]
        public async Task<ActionResult<LogroFacilitadorDto>> ObtenerLogroPorId(int id)
        {
            try
            {
                var logro = await _servicio.ObtenerLogroPorIdAsync(id);
                if (logro == null)
                {
                    return NotFound(new { mensaje = $"Logro con ID {id} no encontrado" });
                }

                return Ok(logro);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener logro {id}: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene todos los logros con filtros opcionales
        /// </summary>
        [HttpPost("logros/buscar")]
        public async Task<ActionResult<List<LogroFacilitadorDto>>> BuscarLogros([FromBody] FiltrosLogrosDto? filtros)
        {
            try
            {
                var logros = await _servicio.ObtenerTodosLosLogrosAsync(filtros);
                return Ok(logros);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al buscar logros: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza un logro (Solo Coordinadores)
        /// </summary>
        [HttpPut("logros/{id}")]
        public async Task<ActionResult<LogroFacilitadorDto>> ActualizarLogro(int id, [FromBody] ActualizarLogroFacilitadorDto dto)
        {
            try
            {
                // Verificar que es coordinador
                var roleClaim = User.FindFirst("RoleId")?.Value;
                if (string.IsNullOrEmpty(roleClaim) || roleClaim != "5")
                {
                    return StatusCode(403, new { mensaje = "Solo los coordinadores pueden actualizar logros" });
                }

                var resultado = await _servicio.ActualizarLogroAsync(id, dto);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar logro {id}: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina un logro (Solo Coordinadores)
        /// </summary>
        [HttpDelete("logros/{id}")]
        public async Task<ActionResult> EliminarLogro(int id)
        {
            try
            {
                // Verificar que es coordinador
                var roleClaim = User.FindFirst("RoleId")?.Value;
                if (string.IsNullOrEmpty(roleClaim) || roleClaim != "5")
                {
                    return StatusCode(403, new { mensaje = "Solo los coordinadores pueden eliminar logros" });
                }

                var resultado = await _servicio.EliminarLogroAsync(id);
                if (!resultado)
                {
                    return NotFound(new { mensaje = $"Logro con ID {id} no encontrado" });
                }

                return Ok(new { mensaje = "Logro eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar logro {id}: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Otorga un logro a un facilitador (Solo Coordinadores)
        /// </summary>
        [HttpPost("otorgar")]
        public async Task<ActionResult<LogroObtenidoFacilitadorDto>> OtorgarLogro([FromBody] OtorgarLogroDto dto)
        {
            try
            {
                // Verificar que es coordinador
                var roleClaim = User.FindFirst("RoleId")?.Value;
                if (string.IsNullOrEmpty(roleClaim) || roleClaim != "5")
                {
                    return StatusCode(403, new { mensaje = "Solo los coordinadores pueden otorgar logros" });
                }

                var usuarioIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(usuarioIdClaim) || !int.TryParse(usuarioIdClaim, out int usuarioId))
                {
                    return StatusCode(403, new { mensaje = "No se pudo identificar al usuario" });
                }

                var resultado = await _servicio.OtorgarLogroAsync(dto, usuarioId);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al otorgar logro: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Revoca un logro obtenido (Solo Coordinadores)
        /// </summary>
        [HttpDelete("revocar/{idLogroObtenido}")]
        public async Task<ActionResult> RevocarLogro(int idLogroObtenido)
        {
            try
            {
                // Verificar que es coordinador
                var roleClaim = User.FindFirst("RoleId")?.Value;
                if (string.IsNullOrEmpty(roleClaim) || roleClaim != "5")
                {
                    return StatusCode(403, new { mensaje = "Solo los coordinadores pueden revocar logros" });
                }

                var usuarioIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(usuarioIdClaim) || !int.TryParse(usuarioIdClaim, out int usuarioId))
                {
                    return StatusCode(403, new { mensaje = "No se pudo identificar al usuario" });
                }

                var resultado = await _servicio.RevocarLogroAsync(idLogroObtenido, usuarioId);
                if (!resultado)
                {
                    return NotFound(new { mensaje = $"Logro obtenido con ID {idLogroObtenido} no encontrado" });
                }

                return Ok(new { mensaje = "Logro revocado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al revocar logro {idLogroObtenido}: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene logros obtenidos con filtros
        /// </summary>
        [HttpPost("obtenidos/buscar")]
        public async Task<ActionResult<List<LogroObtenidoFacilitadorDto>>> BuscarLogrosObtenidos([FromBody] FiltrosLogrosObtenidosDto? filtros)
        {
            try
            {
                var logros = await _servicio.ObtenerLogrosObtenidosAsync(filtros);
                return Ok(logros);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al buscar logros obtenidos: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el perfil de logros de un facilitador
        /// </summary>
        [HttpGet("facilitador/{facilitadorId}/perfil")]
        public async Task<ActionResult<PerfilLogrosFacilitadorDto>> ObtenerPerfilFacilitador(int facilitadorId)
        {
            try
            {
                var perfil = await _servicio.ObtenerPerfilFacilitadorAsync(facilitadorId);
                return Ok(perfil);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener perfil del facilitador {facilitadorId}: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el perfil del facilitador autenticado
        /// </summary>
        [HttpGet("mi-perfil")]
        public async Task<ActionResult<PerfilLogrosFacilitadorDto>> ObtenerMiPerfil()
        {
            try
            {
                var usuarioIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(usuarioIdClaim) || !int.TryParse(usuarioIdClaim, out int usuarioId))
                {
                    return StatusCode(403, new { mensaje = "No se pudo identificar al usuario" });
                }

                var perfil = await _servicio.ObtenerPerfilFacilitadorAsync(usuarioId);
                return Ok(perfil);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener perfil propio: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el ranking de facilitadores
        /// </summary>
        [HttpGet("ranking")]
        public async Task<ActionResult<List<RankingFacilitadorDto>>> ObtenerRanking([FromQuery] int top = 10)
        {
            try
            {
                var ranking = await _servicio.ObtenerRankingFacilitadoresAsync(top);
                return Ok(ranking);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener ranking: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene estadísticas generales de logros
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<EstadisticasLogrosDto>> ObtenerEstadisticas()
        {
            try
            {
                var estadisticas = await _servicio.ObtenerEstadisticasGeneralesAsync();
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener estadísticas: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }
    }
}
