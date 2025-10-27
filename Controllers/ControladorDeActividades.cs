using ApiPG.DTOs;
using ApiPG.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiPG.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ActividadesController : ControllerBase
    {
        private readonly IServicioDeActividad _servicio;

        public ActividadesController(IServicioDeActividad servicio)
        {
            _servicio = servicio;
        }

        /// <summary>
        /// Crear una nueva actividad (solo coordinadores)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ActividadDto>> CrearActividad([FromBody] CrearActividadDto dto)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var actividad = await _servicio.CrearActividadAsync(dto, usuarioId);
                return CreatedAtAction(nameof(ObtenerActividadPorId), new { id = actividad.Id }, actividad);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear la actividad", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtener una actividad por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ActividadDto>> ObtenerActividadPorId(int id)
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
                return StatusCode(500, new { mensaje = "Error al obtener la actividad", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtener todas las actividades
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActividadDto>>> ObtenerTodasLasActividades()
        {
            try
            {
                var actividades = await _servicio.ObtenerTodasLasActividadesAsync();
                return Ok(actividades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las actividades", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtener actividades creadas por el coordinador actual
        /// </summary>
        [HttpGet("mis-actividades")]
        public async Task<ActionResult<IEnumerable<ActividadDto>>> ObtenerMisActividades()
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var actividades = await _servicio.ObtenerActividadesPorCoordinadorAsync(usuarioId);
                return Ok(actividades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las actividades", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtener actividades asignadas al voluntario actual
        /// </summary>
        [HttpGet("asignadas")]
        public async Task<ActionResult<IEnumerable<ActividadDto>>> ObtenerActividadesAsignadas()
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var actividades = await _servicio.ObtenerActividadesDeVoluntarioAsync(usuarioId);
                return Ok(actividades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las actividades asignadas", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtener actividades por rango de fechas
        /// </summary>
        [HttpGet("por-fecha")]
        public async Task<ActionResult<IEnumerable<ActividadDto>>> ObtenerActividadesPorFecha(
            [FromQuery] DateTime fechaInicio, 
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                var actividades = await _servicio.ObtenerActividadesPorFechaAsync(fechaInicio, fechaFin);
                return Ok(actividades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las actividades", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar una actividad
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ActividadDto>> ActualizarActividad(int id, [FromBody] ActualizarActividadDto dto)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var actividad = await _servicio.ActualizarActividadAsync(id, dto, usuarioId);
                return Ok(actividad);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la actividad", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Eliminar una actividad (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarActividad(int id)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var resultado = await _servicio.EliminarActividadAsync(id, usuarioId);
                
                if (!resultado)
                    return NotFound(new { mensaje = "Actividad no encontrada" });

                return Ok(new { mensaje = "Actividad eliminada correctamente" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar la actividad", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtener estadísticas generales de actividades
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<EstadisticasActividadesDto>> ObtenerEstadisticas()
        {
            try
            {
                var estadisticas = await _servicio.ObtenerEstadisticasAsync();
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las estadísticas", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtener estadísticas de actividades del coordinador actual
        /// </summary>
        [HttpGet("mis-estadisticas")]
        public async Task<ActionResult<EstadisticasActividadesDto>> ObtenerMisEstadisticas()
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var estadisticas = await _servicio.ObtenerEstadisticasAsync(coordinadorId: usuarioId);
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las estadísticas", detalle = ex.Message });
            }
        }
    }
}
