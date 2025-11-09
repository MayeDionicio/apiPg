using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiPG.Services;
using ApiPG.DTOs;
using System.Security.Claims;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/actividades-montessori")]
    public class ControladorDeActividadesMontessori : ControllerBase
    {
        private readonly IServicioDeActividadMontessori _servicio;
        private readonly ILogger<ControladorDeActividadesMontessori> _logger;

        public ControladorDeActividadesMontessori(
            IServicioDeActividadMontessori servicio,
            ILogger<ControladorDeActividadesMontessori> logger)
        {
            _servicio = servicio;
            _logger = logger;
        }

        /// <summary>
        /// Crear una nueva actividad Montessori
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ActividadMontessoriDto>> CrearActividad([FromBody] CrearActividadMontessoriDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int voluntarioId))
                    return Unauthorized(new { mensaje = "Usuario no autenticado" });

                var actividad = await _servicio.CrearActividadAsync(dto, voluntarioId);
                return Ok(actividad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear actividad Montessori");
                return StatusCode(500, new { mensaje = "Error al crear la actividad", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtener todas las actividades Montessori
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActividadMontessoriDto>>> ObtenerTodas()
        {
            try
            {
                var actividades = await _servicio.ObtenerTodasLasActividadesAsync();
                return Ok(actividades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividades Montessori");
                return StatusCode(500, new { mensaje = "Error al obtener las actividades" });
            }
        }

        /// <summary>
        /// Obtener una actividad Montessori por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ActividadMontessoriDto>> ObtenerPorId(int id)
        {
            try
            {
                var actividad = await _servicio.ObtenerActividadPorIdAsync(id);
                if (actividad == null)
                    return NotFound(new { mensaje = "Actividad no encontrada" });

                return Ok(actividad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividad {Id}", id);
                return StatusCode(500, new { mensaje = "Error al obtener la actividad" });
            }
        }

        /// <summary>
        /// Obtener actividades Montessori por área pedagógica
        /// </summary>
        [HttpGet("area/{area}")]
        public async Task<ActionResult<IEnumerable<ActividadMontessoriDto>>> ObtenerPorArea(string area)
        {
            try
            {
                var actividades = await _servicio.ObtenerActividadesPorAreaAsync(area);
                return Ok(actividades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividades por área {Area}", area);
                return StatusCode(500, new { mensaje = "Error al obtener las actividades" });
            }
        }

        /// <summary>
        /// Obtener actividades Montessori creadas por un voluntario
        /// </summary>
        [HttpGet("voluntario/{voluntarioId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ActividadMontessoriDto>>> ObtenerPorVoluntario(int voluntarioId)
        {
            try
            {
                var actividades = await _servicio.ObtenerActividadesPorVoluntarioAsync(voluntarioId);
                return Ok(actividades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividades del voluntario {VoluntarioId}", voluntarioId);
                return StatusCode(500, new { mensaje = "Error al obtener las actividades" });
            }
        }

        /// <summary>
        /// Obtener actividades Montessori para un participante según su edad
        /// </summary>
        [HttpGet("participante/{participanteId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ActividadMontessoriDto>>> ObtenerParaParticipante(int participanteId)
        {
            try
            {
                var actividades = await _servicio.ObtenerActividadesParaParticipanteAsync(participanteId);
                return Ok(actividades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividades para participante {ParticipanteId}", participanteId);
                return StatusCode(500, new { mensaje = "Error al obtener las actividades" });
            }
        }

        /// <summary>
        /// Obtener actividades Montessori para un nivel según las edades de sus participantes
        /// </summary>
        [HttpGet("nivel/{nivelId}")]
        public async Task<ActionResult<IEnumerable<ActividadMontessoriDto>>> ObtenerPorNivel(int nivelId)
        {
            try
            {
                var actividades = await _servicio.ObtenerActividadesPorNivelAsync(nivelId);
                return Ok(actividades);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividades para nivel {NivelId}", nivelId);
                return StatusCode(500, new { mensaje = "Error al obtener las actividades" });
            }
        }

        /// <summary>
        /// Actualizar una actividad Montessori
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ActividadMontessoriDto>> ActualizarActividad(int id, [FromBody] ActualizarActividadMontessoriDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int voluntarioId))
                    return Unauthorized(new { mensaje = "Usuario no autenticado" });

                var actividad = await _servicio.ActualizarActividadAsync(id, dto, voluntarioId);
                return Ok(actividad);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new { mensaje = "No tiene permiso para actualizar esta actividad" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { mensaje = "Actividad no encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar actividad {Id}", id);
                return StatusCode(500, new { mensaje = "Error al actualizar la actividad" });
            }
        }

        /// <summary>
        /// Eliminar una actividad Montessori (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> EliminarActividad(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int voluntarioId))
                    return Unauthorized(new { mensaje = "Usuario no autenticado" });

                var resultado = await _servicio.EliminarActividadAsync(id, voluntarioId);
                if (!resultado)
                    return NotFound(new { mensaje = "Actividad no encontrada o ya está inactiva" });

                return Ok(new { mensaje = "Actividad eliminada correctamente" });
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new { mensaje = "No tiene permiso para eliminar esta actividad" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar actividad {Id}", id);
                return StatusCode(500, new { mensaje = "Error al eliminar la actividad" });
            }
        }

        /// <summary>
        /// Obtener estadísticas de actividades Montessori
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<EstadisticasActividadesMontessoriDto>> ObtenerEstadisticas([FromQuery] int? voluntarioId = null)
        {
            try
            {
                var estadisticas = await _servicio.ObtenerEstadisticasAsync(voluntarioId);
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas");
                return StatusCode(500, new { mensaje = "Error al obtener las estadísticas" });
            }
        }

        // ============ ENDPOINTS PARA LOGROS ============

        /// <summary>
        /// Crear un nuevo logro Montessori
        /// </summary>
        [HttpPost("logros")]
        [Authorize]
        public async Task<ActionResult<LogroMontessoriDto>> CrearLogro([FromBody] CrearLogroMontessoriDto dto)
        {
            try
            {
                var logro = await _servicio.CrearLogroAsync(dto);
                return Ok(logro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear logro");
                return StatusCode(500, new { mensaje = "Error al crear el logro", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Marcar un logro como obtenido
        /// </summary>
        [HttpPatch("logros/{logroId}/marcar-obtenido")]
        [Authorize]
        public async Task<ActionResult<LogroMontessoriDto>> MarcarLogroObtenido(int logroId)
        {
            try
            {
                var logro = await _servicio.MarcarLogroComoObtenidoAsync(logroId);
                return Ok(logro);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { mensaje = "Logro no encontrado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar logro {LogroId} como obtenido", logroId);
                return StatusCode(500, new { mensaje = "Error al marcar el logro" });
            }
        }

        /// <summary>
        /// Obtener logros de una actividad
        /// </summary>
        [HttpGet("{actividadId}/logros")]
        public async Task<ActionResult<IEnumerable<LogroMontessoriDto>>> ObtenerLogrosPorActividad(int actividadId)
        {
            try
            {
                var logros = await _servicio.ObtenerLogrosPorActividadAsync(actividadId);
                return Ok(logros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener logros de actividad {ActividadId}", actividadId);
                return StatusCode(500, new { mensaje = "Error al obtener los logros" });
            }
        }

        /// <summary>
        /// Obtener logros de un usuario
        /// </summary>
        [HttpGet("logros/usuario/{usuarioId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LogroMontessoriDto>>> ObtenerLogrosPorUsuario(int usuarioId)
        {
            try
            {
                var logros = await _servicio.ObtenerLogrosPorUsuarioAsync(usuarioId);
                return Ok(logros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener logros del usuario {UsuarioId}", usuarioId);
                return StatusCode(500, new { mensaje = "Error al obtener los logros" });
            }
        }

        /// <summary>
        /// Eliminar un logro
        /// </summary>
        [HttpDelete("logros/{logroId}")]
        [Authorize]
        public async Task<ActionResult> EliminarLogro(int logroId)
        {
            try
            {
                var resultado = await _servicio.EliminarLogroAsync(logroId);
                if (!resultado)
                    return NotFound(new { mensaje = "Logro no encontrado" });

                return Ok(new { mensaje = "Logro eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar logro {LogroId}", logroId);
                return StatusCode(500, new { mensaje = "Error al eliminar el logro" });
            }
        }
    }
}
