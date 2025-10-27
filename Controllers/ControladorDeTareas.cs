using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiPG.DTOs;
using ApiPG.Services;
using System.Security.Claims;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TareasController : ControllerBase
    {
        private readonly IServicioDeTarea _service;

        public TareasController(IServicioDeTarea service)
        {
            _service = service;
        }

        private int ObtenerUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        /// <summary>
        /// Crear una nueva tarea (Solo coordinadores)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TareaDto>> CrearTarea([FromBody] CrearTareaDto dto)
        {
            try
            {
                var coordinadorId = ObtenerUsuarioId();
                var tarea = await _service.CrearTareaAsync(dto, coordinadorId);
                return CreatedAtAction(nameof(ObtenerTareaPorId), new { id = tarea.Id }, tarea);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener tarea por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TareaDto>> ObtenerTareaPorId(int id)
        {
            var tarea = await _service.ObtenerTareaPorIdAsync(id);
            
            if (tarea == null)
                return NotFound(new { message = "Tarea no encontrada" });

            return Ok(tarea);
        }

        /// <summary>
        /// Obtener todas las tareas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TareaDto>>> ObtenerTodasLasTareas()
        {
            var tareas = await _service.ObtenerTodasLasTareasAsync();
            return Ok(tareas);
        }

        /// <summary>
        /// Obtener tareas creadas por el coordinador actual
        /// </summary>
        [HttpGet("mis-tareas")]
        public async Task<ActionResult<IEnumerable<TareaDto>>> ObtenerMisTareas()
        {
            var coordinadorId = ObtenerUsuarioId();
            var tareas = await _service.ObtenerTareasPorCoordinadorAsync(coordinadorId);
            return Ok(tareas);
        }

        /// <summary>
        /// Obtener tareas asignadas al voluntario actual
        /// </summary>
        [HttpGet("asignadas")]
        public async Task<ActionResult<IEnumerable<TareaDto>>> ObtenerTareasAsignadas()
        {
            var voluntarioId = ObtenerUsuarioId();
            var tareas = await _service.ObtenerTareasDeVoluntarioAsync(voluntarioId);
            return Ok(tareas);
        }

        /// <summary>
        /// Actualizar una tarea (Solo coordinador que la creó)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TareaDto>> ActualizarTarea(int id, [FromBody] ActualizarTareaDto dto)
        {
            try
            {
                var coordinadorId = ObtenerUsuarioId();
                var tarea = await _service.ActualizarTareaAsync(id, dto, coordinadorId);
                return Ok(tarea);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Eliminar (desactivar) una tarea (Solo coordinador que la creó)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarTarea(int id)
        {
            try
            {
                var coordinadorId = ObtenerUsuarioId();
                var resultado = await _service.EliminarTareaAsync(id, coordinadorId);
                
                if (!resultado)
                    return NotFound(new { message = "Tarea no encontrada" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        /// <summary>
        /// Actualizar estado de la tarea por el voluntario (Vista, EnProceso, Completada)
        /// </summary>
        [HttpPatch("{id}/estado")]
        public async Task<ActionResult<TareaDto>> ActualizarEstado(int id, [FromBody] ActualizarEstadoTareaDto dto)
        {
            try
            {
                var voluntarioId = ObtenerUsuarioId();
                var tarea = await _service.ActualizarEstadoTareaAsync(id, voluntarioId, dto);
                return Ok(tarea);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Aprobar o rechazar una tarea (Solo coordinador que la creó)
        /// </summary>
        [HttpPost("{id}/revisar")]
        public async Task<ActionResult<TareaDto>> RevisarTarea(int id, [FromBody] RevisarTareaDto dto)
        {
            try
            {
                var coordinadorId = ObtenerUsuarioId();
                var tarea = await _service.RevisarTareaAsync(id, dto, coordinadorId);
                return Ok(tarea);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener estadísticas de tareas
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<EstadisticasTareasDto>> ObtenerEstadisticas(
            [FromQuery] int? coordinadorId = null, 
            [FromQuery] int? voluntarioId = null)
        {
            var estadisticas = await _service.ObtenerEstadisticasAsync(coordinadorId, voluntarioId);
            return Ok(estadisticas);
        }

        /// <summary>
        /// Obtener estadísticas de mis tareas como coordinador
        /// </summary>
        [HttpGet("mis-estadisticas")]
        public async Task<ActionResult<EstadisticasTareasDto>> ObtenerMisEstadisticas()
        {
            var coordinadorId = ObtenerUsuarioId();
            var estadisticas = await _service.ObtenerEstadisticasAsync(coordinadorId: coordinadorId);
            return Ok(estadisticas);
        }

        /// <summary>
        /// Obtener estadísticas de tareas asignadas como voluntario
        /// </summary>
        [HttpGet("mis-estadisticas-voluntario")]
        public async Task<ActionResult<EstadisticasTareasDto>> ObtenerMisEstadisticasComoVoluntario()
        {
            var voluntarioId = ObtenerUsuarioId();
            var estadisticas = await _service.ObtenerEstadisticasAsync(voluntarioId: voluntarioId);
            return Ok(estadisticas);
        }
    }
}
