using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiPG.Services;
using ApiPG.DTOs;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/analisis")]
    [Authorize]
    public class ControladorDeAnalisis : ControllerBase
    {
        private readonly IServicioDeAnalisis _analisisService;

        public ControladorDeAnalisis(IServicioDeAnalisis analisisService)
        {
            _analisisService = analisisService;
        }

        /// <summary>
        /// Obtener métricas generales del dashboard
        /// </summary>
        [HttpGet("metricas-generales")]
        public async Task<ActionResult<MetricasGeneralesDto>> ObtenerMetricasGenerales()
        {
            try
            {
                var metricas = await _analisisService.ObtenerMetricasGeneralesAsync();
                return Ok(metricas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener métricas generales", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener datos de crecimiento de usuarios
        /// </summary>
        [HttpGet("crecimiento-usuarios")]
        public async Task<ActionResult<CrecimientoUsuariosDto>> ObtenerCrecimientoUsuarios([FromQuery] int meses = 6)
        {
            try
            {
                var datos = await _analisisService.ObtenerCrecimientoUsuariosAsync(meses);
                return Ok(datos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener crecimiento de usuarios", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener estadísticas de actividades completadas
        /// </summary>
        [HttpGet("actividades-completadas")]
        public async Task<ActionResult<ActividadesCompletadasDto>> ObtenerActividadesCompletadas()
        {
            try
            {
                var datos = await _analisisService.ObtenerActividadesCompletadasAsync();
                return Ok(datos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener actividades completadas", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener estadísticas de tiempo de uso
        /// </summary>
        [HttpGet("tiempo-uso")]
        public async Task<ActionResult<TiempoDeUsoDto>> ObtenerTiempoDeUso()
        {
            try
            {
                var datos = await _analisisService.ObtenerTiempoDeUsoAsync();
                return Ok(datos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener tiempo de uso", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener estadísticas detalladas del sistema
        /// </summary>
        [HttpGet("estadisticas-detalladas")]
        public async Task<ActionResult<EstadisticasDetalladasDto>> ObtenerEstadisticasDetalladas()
        {
            try
            {
                var datos = await _analisisService.ObtenerEstadisticasDetalladasAsync();
                return Ok(datos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener estadísticas detalladas", error = ex.Message });
            }
        }
    }
}
