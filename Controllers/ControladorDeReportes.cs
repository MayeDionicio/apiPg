using ApiPG.DTOs;
using ApiPG.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControladorDeReportes : ControllerBase
    {
        private readonly IServicioDeReportes _servicio;

        public ControladorDeReportes(IServicioDeReportes servicio)
        {
            _servicio = servicio;
        }

        /// <summary>
        /// Genera un reporte general completo con todas las estadísticas del sistema
        /// Disponible para Facilitadores, Coordinadores y Administradores
        /// </summary>
        [HttpGet("general")]
        public async Task<ActionResult<ReporteGeneralDto>> ObtenerReporteGeneral([FromQuery] FiltrosReporteDto? filtros)
        {
            var reporte = await _servicio.GenerarReporteGeneralAsync(filtros);
            return Ok(reporte);
        }

        /// <summary>
        /// Obtiene el resumen de usuarios (voluntarios, participantes, coordinadores)
        /// </summary>
        [HttpGet("usuarios")]
        public async Task<ActionResult<ResumenUsuariosDto>> ObtenerResumenUsuarios()
        {
            var resumen = await _servicio.ObtenerResumenUsuariosAsync();
            return Ok(resumen);
        }

        /// <summary>
        /// Obtiene el resumen de actividades regulares y Montessori
        /// </summary>
        [HttpGet("actividades")]
        public async Task<ActionResult<ResumenActividadesDto>> ObtenerResumenActividades()
        {
            var resumen = await _servicio.ObtenerResumenActividadesAsync();
            return Ok(resumen);
        }

        /// <summary>
        /// Obtiene el resumen de niveles con sus participantes
        /// </summary>
        [HttpGet("niveles")]
        public async Task<ActionResult<ResumenNivelesDto>> ObtenerResumenNiveles()
        {
            var resumen = await _servicio.ObtenerResumenNivelesAsync();
            return Ok(resumen);
        }

        /// <summary>
        /// Obtiene el resumen de asistencia con filtros opcionales de fecha
        /// </summary>
        [HttpGet("asistencia")]
        public async Task<ActionResult<ResumenAsistenciaDto>> ObtenerResumenAsistencia(
            [FromQuery] DateTime? fechaInicio, 
            [FromQuery] DateTime? fechaFin)
        {
            var resumen = await _servicio.ObtenerResumenAsistenciaAsync(fechaInicio, fechaFin);
            return Ok(resumen);
        }

        /// <summary>
        /// Obtiene el resumen de tareas por estado y voluntario
        /// </summary>
        [HttpGet("tareas")]
        public async Task<ActionResult<ResumenTareasDto>> ObtenerResumenTareas()
        {
            var resumen = await _servicio.ObtenerResumenTareasAsync();
            return Ok(resumen);
        }
    }
}
