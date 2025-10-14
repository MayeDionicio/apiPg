using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiPG.Services;
using ApiPG.DTOs;
using ApiPG.Models;
using System.Security.Claims;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DevocionalController : ControllerBase
    {
        private readonly IDevocionalService _devocionalService;

        public DevocionalController(IDevocionalService devocionalService)
        {
            _devocionalService = devocionalService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        /// <summary>
        /// Obtener todos los devocionales
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DevocionalDto>>> GetDevocionales()
        {
            try
            {
                var devocionales = await _devocionalService.GetAllAsync();
                return Ok(devocionales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener un devocional por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DevocionalDto>> GetDevocional(int id)
        {
            try
            {
                var devocional = await _devocionalService.GetByIdAsync(id);
                if (devocional == null)
                {
                    return NotFound(new { message = $"Devocional con ID {id} no encontrado" });
                }
                return Ok(devocional);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Crear un nuevo devocional completo
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DevocionalDto>> CreateDevocional([FromBody] CrearDevocionalDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetCurrentUserId();
                var devocional = await _devocionalService.CreateAsync(createDto, userId);
                
                return CreatedAtAction(nameof(GetDevocional), new { id = devocional.Id }, devocional);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar un devocional
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<DevocionalDto>> UpdateDevocional(int id, [FromBody] ActualizarDevocionalDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetCurrentUserId();
                var devocional = await _devocionalService.UpdateAsync(id, updateDto, userId);
                
                if (devocional == null)
                {
                    return NotFound(new { message = $"Devocional con ID {id} no encontrado" });
                }

                return Ok(devocional);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Desactivar un devocional
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDevocional(int id)
        {
            try
            {
                var result = await _devocionalService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Devocional con ID {id} no encontrado" });
                }

                return Ok(new { message = "Devocional desactivado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Activar un devocional
        /// </summary>
        [HttpPut("{id}/activate")]
        public async Task<ActionResult> ActivateDevocional(int id)
        {
            try
            {
                var result = await _devocionalService.ActivateAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Devocional con ID {id} no encontrado" });
                }

                return Ok(new { message = "Devocional activado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // ENDPOINTS ESPECÍFICOS DEL WIZARD

        /// <summary>
        /// Guardar paso 1: Información básica
        /// </summary>
        [HttpPost("asistente/paso1")]
        public async Task<ActionResult<DevocionalDto>> SaveStep1([FromBody] DevocionalPaso1Dto step1Dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetCurrentUserId();
                var devocional = await _devocionalService.SaveStep1Async(step1Dto, userId);
                
                return Ok(devocional);
            }
            catch (Exception ex)
            {
                // Log detallado para debugging
                var innerException = ex.InnerException?.Message ?? "No inner exception";
                var stackTrace = ex.StackTrace ?? "No stack trace";
                
                return StatusCode(500, new { 
                    message = "Error al guardar paso 1", 
                    error = ex.Message,
                    innerError = innerException,
                    stackTrace = stackTrace
                });
            }
        }

        /// <summary>
        /// Guardar paso 2: Texto bíblico
        /// </summary>
        [HttpPut("asistente/{id}/paso2")]
        public async Task<ActionResult<DevocionalDto>> SaveStep2(int id, [FromBody] DevocionalPaso2Dto step2Dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetCurrentUserId();
                var devocional = await _devocionalService.SaveStep2Async(id, step2Dto, userId);
                
                return Ok(devocional);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al guardar paso 2", error = ex.Message });
            }
        }

        /// <summary>
        /// Guardar paso 3: Contenido
        /// </summary>
        [HttpPut("asistente/{id}/paso3")]
        public async Task<ActionResult<DevocionalDto>> SaveStep3(int id, [FromBody] DevocionalPaso3Dto step3Dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var devocional = await _devocionalService.SaveStep3Async(id, step3Dto, userId);
                
                return Ok(devocional);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al guardar paso 3", error = ex.Message });
            }
        }

        /// <summary>
        /// Guardar como borrador
        /// </summary>
        [HttpPost("borrador")]
        public async Task<ActionResult<DevocionalDto>> SaveDraft([FromBody] GuardarBorradorDto draftDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var devocional = await _devocionalService.SaveDraftAsync(draftDto, userId);
                
                return Ok(devocional);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al guardar borrador", error = ex.Message });
            }
        }

        /// <summary>
        /// Finalizar devocional (cambiar a Programado)
        /// </summary>
        [HttpPut("{id}/finalizar")]
        public async Task<ActionResult<DevocionalDto>> FinalizeDevocional(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var devocional = await _devocionalService.FinalizeDevocionalAsync(id, userId);
                
                return Ok(devocional);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al finalizar devocional", error = ex.Message });
            }
        }

        // FILTROS Y CONSULTAS

        /// <summary>
        /// Obtener devocionales por estado
        /// </summary>
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<DevocionalDto>>> GetByEstado(DevocionalEstado estado)
        {
            try
            {
                var devocionales = await _devocionalService.GetByEstadoAsync(estado);
                return Ok(devocionales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener devocionales por voluntario
        /// </summary>
        [HttpGet("voluntario/{voluntarioId}")]
        public async Task<ActionResult<IEnumerable<DevocionalDto>>> GetByVoluntario(int voluntarioId)
        {
            try
            {
                var devocionales = await _devocionalService.GetByVoluntarioAsync(voluntarioId);
                return Ok(devocionales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener devocionales por fecha específica
        /// </summary>
        [HttpGet("fecha/{fecha}")]
        public async Task<ActionResult<IEnumerable<DevocionalDto>>> GetByFecha(DateTime fecha)
        {
            try
            {
                var devocionales = await _devocionalService.GetByFechaAsync(fecha);
                return Ok(devocionales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener devocionales en rango de fechas
        /// </summary>
        [HttpGet("rango-fechas")]
        public async Task<ActionResult<IEnumerable<DevocionalDto>>> GetByRangoFecha(
            [FromQuery] DateTime fechaInicio, 
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                var devocionales = await _devocionalService.GetByRangoFechaAsync(fechaInicio, fechaFin);
                return Ok(devocionales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener borradores
        /// </summary>
        [HttpGet("borradores")]
        public async Task<ActionResult<IEnumerable<DevocionalDto>>> GetBorradores()
        {
            try
            {
                var devocionales = await _devocionalService.GetBorradoresAsync();
                return Ok(devocionales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener devocionales programados
        /// </summary>
        [HttpGet("programados")]
        public async Task<ActionResult<IEnumerable<DevocionalDto>>> GetProgramados()
        {
            try
            {
                var devocionales = await _devocionalService.GetProgramadosAsync();
                return Ok(devocionales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener próximos devocionales
        /// </summary>
        [HttpGet("proximos")]
        public async Task<ActionResult<IEnumerable<DevocionalDto>>> GetProximos([FromQuery] int dias = 7)
        {
            try
            {
                var devocionales = await _devocionalService.GetProximosAsync(dias);
                return Ok(devocionales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener devocionales recientes
        /// </summary>
        [HttpGet("recientes")]
        public async Task<ActionResult<IEnumerable<DevocionalDto>>> GetRecientes([FromQuery] int take = 10)
        {
            try
            {
                var devocionales = await _devocionalService.GetRecentesAsync(take);
                return Ok(devocionales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener mis devocionales (creados por el usuario actual)
        /// </summary>
        [HttpGet("mis-devocionales")]
        public async Task<ActionResult<IEnumerable<DevocionalDto>>> GetMisDevocionales()
        {
            try
            {
                var userId = GetCurrentUserId();
                var devocionales = await _devocionalService.GetByCreatedByUserAsync(userId);
                return Ok(devocionales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // CAMBIOS DE ESTADO

        /// <summary>
        /// Cambiar estado de un devocional
        /// </summary>
        [HttpPut("{id}/estado/{nuevoEstado}")]
        public async Task<ActionResult<DevocionalDto>> CambiarEstado(int id, DevocionalEstado nuevoEstado)
        {
            try
            {
                var userId = GetCurrentUserId();
                var devocional = await _devocionalService.CambiarEstadoAsync(id, nuevoEstado, userId);
                
                if (devocional == null)
                {
                    return NotFound(new { message = $"Devocional con ID {id} no encontrado" });
                }

                return Ok(devocional);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al cambiar estado", error = ex.Message });
            }
        }

        /// <summary>
        /// Programar devocional
        /// </summary>
        [HttpPut("{id}/programar")]
        public async Task<ActionResult<DevocionalDto>> ProgramarDevocional(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var devocional = await _devocionalService.ProgrimarDevocionalAsync(id, userId);
                
                if (devocional == null)
                {
                    return NotFound(new { message = $"Devocional con ID {id} no encontrado" });
                }

                return Ok(devocional);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al programar devocional", error = ex.Message });
            }
        }

        /// <summary>
        /// Iniciar devocional
        /// </summary>
        [HttpPut("{id}/iniciar")]
        public async Task<ActionResult<DevocionalDto>> IniciarDevocional(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var devocional = await _devocionalService.IniciarDevocionalAsync(id, userId);
                
                if (devocional == null)
                {
                    return NotFound(new { message = $"Devocional con ID {id} no encontrado" });
                }

                return Ok(devocional);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al iniciar devocional", error = ex.Message });
            }
        }

        /// <summary>
        /// Completar devocional
        /// </summary>
        [HttpPut("{id}/completar")]
        public async Task<ActionResult<DevocionalDto>> CompletarDevocional(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var devocional = await _devocionalService.CompletarDevocionalAsync(id, userId);
                
                if (devocional == null)
                {
                    return NotFound(new { message = $"Devocional con ID {id} no encontrado" });
                }

                return Ok(devocional);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al completar devocional", error = ex.Message });
            }
        }

        /// <summary>
        /// Cancelar devocional
        /// </summary>
        [HttpPut("{id}/cancelar")]
        public async Task<ActionResult<DevocionalDto>> CancelarDevocional(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var devocional = await _devocionalService.CancelarDevocionalAsync(id, userId);
                
                if (devocional == null)
                {
                    return NotFound(new { message = $"Devocional con ID {id} no encontrado" });
                }

                return Ok(devocional);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al cancelar devocional", error = ex.Message });
            }
        }

        // UTILIDADES

        /// <summary>
        /// Obtener preview de un devocional
        /// </summary>
        [HttpGet("{id}/vista-previa")]
        public async Task<ActionResult<DevocionalVistaPreviaDto>> GetPreview(int id)
        {
            try
            {
                var preview = await _devocionalService.GetPreviewAsync(id);
                if (preview == null)
                {
                    return NotFound(new { message = $"Devocional con ID {id} no encontrado" });
                }
                return Ok(preview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener preview", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener lista de voluntarios disponibles
        /// </summary>
        [HttpGet("available-volunteers")]
        public async Task<ActionResult<IEnumerable<string>>> GetVoluntariosDisponibles()
        {
            try
            {
                var voluntarios = await _devocionalService.GetVoluntariosDisponiblesAsync();
                return Ok(voluntarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener voluntarios", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener estadísticas de devocionales
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<DevocionalEstadisticasDto>> GetEstadisticas()
        {
            try
            {
                var estadisticas = await _devocionalService.GetEstadisticasAsync();
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener estadísticas", error = ex.Message });
            }
        }

        /// <summary>
        /// Buscar devocionales
        /// </summary>
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<DevocionalDto>>> Search([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return BadRequest(new { message = "El término de búsqueda es requerido" });
                }

                var devocionales = await _devocionalService.SearchAsync(q);
                return Ok(devocionales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error en la búsqueda", error = ex.Message });
            }
        }
    }
}