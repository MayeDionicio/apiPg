using ApiPG.DTOs;
using ApiPG.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ControladorDeActividadesMontessoriPersonalizadas : ControllerBase
    {
        private readonly IServicioDeActividadMontessoriPersonalizada _servicio;
        private readonly ILogger<ControladorDeActividadesMontessoriPersonalizadas> _logger;

        public ControladorDeActividadesMontessoriPersonalizadas(
            IServicioDeActividadMontessoriPersonalizada servicio,
            ILogger<ControladorDeActividadesMontessoriPersonalizadas> logger)
        {
            _servicio = servicio;
            _logger = logger;
        }

        /// <summary>
        /// Crea una nueva actividad Montessori personalizada para un estudiante
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ActividadPersonalizadaDto>> CrearActividad(
            [FromBody] CrearActividadPersonalizadaDto dto)
        {
            try
            {
                var usuarioIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(usuarioIdClaim) || !int.TryParse(usuarioIdClaim, out int usuarioId))
                {
                    return StatusCode(403, new { mensaje = "No se pudo identificar al usuario" });
                }

                var resultado = await _servicio.CrearActividadPersonalizadaAsync(dto, usuarioId);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear actividad personalizada: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene una actividad personalizada por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ActividadPersonalizadaDto>> ObtenerPorId(int id)
        {
            try
            {
                var actividad = await _servicio.ObtenerActividadPorIdAsync(id);
                if (actividad == null)
                {
                    return NotFound(new { mensaje = $"Actividad con ID {id} no encontrada" });
                }

                return Ok(actividad);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener actividad {id}: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene todas las actividades de un estudiante específico
        /// </summary>
        [HttpGet("estudiante/{estudianteId}")]
        public async Task<ActionResult<List<ActividadPersonalizadaDto>>> ObtenerPorEstudiante(
            int estudianteId,
            [FromQuery] bool soloActivas = true)
        {
            try
            {
                var actividades = await _servicio.ObtenerActividadesPorEstudianteAsync(estudianteId, soloActivas);
                return Ok(actividades);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener actividades del estudiante {estudianteId}: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene actividades con filtros personalizados
        /// </summary>
        [HttpPost("buscar")]
        public async Task<ActionResult<List<ActividadPersonalizadaDto>>> BuscarConFiltros(
            [FromBody] FiltrosActividadPersonalizadaDto filtros)
        {
            try
            {
                var actividades = await _servicio.ObtenerActividadesConFiltrosAsync(filtros);
                return Ok(actividades);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al buscar actividades con filtros: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza información general de una actividad personalizada
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ActividadPersonalizadaDto>> ActualizarActividad(
            int id,
            [FromBody] ActualizarActividadPersonalizadaDto dto)
        {
            try
            {
                var resultado = await _servicio.ActualizarActividadAsync(id, dto);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar actividad {id}: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza el progreso y evaluación de una actividad
        /// </summary>
        [HttpPut("{id}/progreso")]
        public async Task<ActionResult<ActividadPersonalizadaDto>> ActualizarProgreso(
            int id,
            [FromBody] ActualizarProgresoActividadDto dto)
        {
            try
            {
                var resultado = await _servicio.ActualizarProgresoAsync(id, dto);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar progreso de actividad {id}: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina (desactiva) una actividad personalizada
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarActividad(int id)
        {
            try
            {
                var resultado = await _servicio.EliminarActividadAsync(id);
                if (!resultado)
                {
                    return NotFound(new { mensaje = $"Actividad con ID {id} no encontrada" });
                }

                return Ok(new { mensaje = "Actividad eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar actividad {id}: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene estadísticas de actividades de un estudiante
        /// </summary>
        [HttpGet("estudiante/{estudianteId}/estadisticas")]
        public async Task<ActionResult<EstadisticasActividadesEstudianteDto>> ObtenerEstadisticas(int estudianteId)
        {
            try
            {
                var estadisticas = await _servicio.ObtenerEstadisticasEstudianteAsync(estudianteId);
                return Ok(estadisticas);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener estadísticas del estudiante {estudianteId}: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene todas las actividades pendientes o en progreso
        /// </summary>
        [HttpGet("pendientes")]
        public async Task<ActionResult<List<ActividadPersonalizadaDto>>> ObtenerPendientes()
        {
            try
            {
                var actividades = await _servicio.ObtenerActividadesPendientesAsync();
                return Ok(actividades);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener actividades pendientes: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene actividades que requieren envío de recordatorio
        /// </summary>
        [HttpGet("recordatorios")]
        public async Task<ActionResult<List<ActividadPersonalizadaDto>>> ObtenerConRecordatorios()
        {
            try
            {
                var actividades = await _servicio.ObtenerActividadesConRecordatorioAsync();
                return Ok(actividades);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener actividades con recordatorios: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }
    }
}
