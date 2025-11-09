using ApiPG.DTOs;
using ApiPG.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ControladorDeReportesVoluntario : ControllerBase
    {
        private readonly IServicioDeReportesVoluntario _servicio;

        public ControladorDeReportesVoluntario(IServicioDeReportesVoluntario servicio)
        {
            _servicio = servicio;
        }

        /// <summary>
        /// Obtiene el reporte completo del voluntario con su nivel y estadísticas de asistencia
        /// Solo accesible por el voluntario autenticado
        /// </summary>
        [HttpGet("mi-reporte")]
        public async Task<ActionResult<ReporteVoluntarioDto>> ObtenerMiReporte([FromQuery] FiltrosReporteVoluntarioDto? filtros)
        {
            // Obtener el ID del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario" });
            }

            var reporte = await _servicio.GenerarReporteDeVoluntarioAsync(userId, filtros);
            
            if (reporte == null)
            {
                return NotFound(new { mensaje = "No tienes un nivel asignado o no eres voluntario" });
            }

            return Ok(reporte);
        }

        /// <summary>
        /// Obtiene solo las estadísticas de asistencia del nivel del voluntario
        /// </summary>
        [HttpGet("estadisticas-asistencia")]
        public async Task<ActionResult<EstadisticasAsistenciaVoluntarioDto>> ObtenerEstadisticasAsistencia()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario" });
            }

            var estadisticas = await _servicio.ObtenerEstadisticasAsistenciaAsync(userId);
            
            if (estadisticas == null)
            {
                return NotFound(new { mensaje = "No tienes un nivel asignado" });
            }

            return Ok(estadisticas);
        }

        /// <summary>
        /// Obtiene la lista de participantes del nivel con sus estadísticas de asistencia
        /// </summary>
        [HttpGet("mis-participantes")]
        public async Task<ActionResult<List<ParticipanteDelNivelDto>>> ObtenerMisParticipantes()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario" });
            }

            var participantes = await _servicio.ObtenerParticipantesConAsistenciaAsync(userId);
            
            return Ok(participantes);
        }

        /// <summary>
        /// Obtiene todas las asistencias registradas en el nivel del voluntario
        /// Permite filtrar por fecha y participante específico
        /// </summary>
        [HttpGet("asistencias-detalladas")]
        public async Task<ActionResult<List<AsistenciaDetalladaVoluntarioDto>>> ObtenerAsistenciasDetalladas(
            [FromQuery] FiltrosReporteVoluntarioDto? filtros)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario" });
            }

            var asistencias = await _servicio.ObtenerAsistenciasDetalladasAsync(userId, filtros);
            
            return Ok(asistencias);
        }
    }
}
