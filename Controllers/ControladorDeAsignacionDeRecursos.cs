using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiPG.Services;
using ApiPG.DTOs;
using System.Security.Claims;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/asignaciones-recursos")]
    [Authorize]
    public class AsignacionesRecursosController : ControllerBase
    {
        private readonly IResourceAssignmentService _assignmentService;

        public AsignacionesRecursosController(IResourceAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        /// <summary>
        /// Obtener todas las asignaciones de recursos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AsignacionDeRecursoDto>>> GetAssignments()
        {
            try
            {
                var assignments = await _assignmentService.GetAllAsync();
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las asignaciones", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener una asignación por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AsignacionDeRecursoDto>> GetAssignment(int id)
        {
            try
            {
                var assignment = await _assignmentService.GetByIdAsync(id);
                if (assignment == null)
                {
                    return NotFound(new { message = $"Asignación con ID {id} no encontrada" });
                }
                return Ok(assignment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la asignación", error = ex.Message });
            }
        }

        /// <summary>
        /// Crear una nueva asignación de recurso
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AsignacionDeRecursoDto>> CreateAssignment([FromBody] CrearAsignacionDeRecursoDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = GetCurrentUserId();
                var assignment = await _assignmentService.CreateAsync(createDto, currentUserId);
                return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, assignment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear la asignación", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar una asignación existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<AsignacionDeRecursoDto>> UpdateAssignment(int id, [FromBody] ActualizarAsignacionDeRecursoDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var assignment = await _assignmentService.UpdateAsync(id, updateDto);
                if (assignment == null)
                {
                    return NotFound(new { message = $"Asignación con ID {id} no encontrada" });
                }

                return Ok(assignment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la asignación", error = ex.Message });
            }
        }

        /// <summary>
        /// Cancelar una asignación
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelAssignment(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var assignment = await _assignmentService.CancelAssignmentAsync(id, currentUserId);
                if (assignment == null)
                {
                    return NotFound(new { message = $"Asignación con ID {id} no encontrada" });
                }

                return Ok(new { message = "Asignación cancelada correctamente", assignment });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al cancelar la asignación", error = ex.Message });
            }
        }

        /// <summary>
        /// Confirmar una asignación (por parte del voluntario)
        /// </summary>
        [HttpPut("{id}/confirmar")]
        public async Task<ActionResult<AsignacionDeRecursoDto>> ConfirmAssignment(int id, [FromBody] ConfirmarAsignacionDto confirmDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = GetCurrentUserId();
                var assignment = await _assignmentService.ConfirmAssignmentAsync(id, currentUserId, confirmDto);
                if (assignment == null)
                {
                    return NotFound(new { message = $"Asignación con ID {id} no encontrada" });
                }

                return Ok(assignment);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al confirmar la asignación", error = ex.Message });
            }
        }

        /// <summary>
        /// Iniciar el uso de un recurso
        /// </summary>
        [HttpPut("{id}/iniciar-uso")]
        public async Task<ActionResult<AsignacionDeRecursoDto>> StartUse(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var assignment = await _assignmentService.StartUseAsync(id, currentUserId);
                if (assignment == null)
                {
                    return NotFound(new { message = $"Asignación con ID {id} no encontrada" });
                }

                return Ok(assignment);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al iniciar el uso", error = ex.Message });
            }
        }

        /// <summary>
        /// Devolver un recurso
        /// </summary>
        [HttpPut("{id}/devolver")]
        public async Task<ActionResult<AsignacionDeRecursoDto>> ReturnResource(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var assignment = await _assignmentService.ReturnResourceAsync(id, currentUserId);
                if (assignment == null)
                {
                    return NotFound(new { message = $"Asignación con ID {id} no encontrada" });
                }

                return Ok(assignment);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al devolver el recurso", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener asignaciones de un voluntario específico
        /// </summary>
        [HttpGet("voluntario/{voluntarioId}")]
        public async Task<ActionResult<IEnumerable<AsignacionDeRecursoDto>>> GetAssignmentsByVolunteer(int voluntarioId)
        {
            try
            {
                var assignments = await _assignmentService.GetByVolunteerAsync(voluntarioId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener asignaciones del voluntario", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener asignaciones de un recurso específico
        /// </summary>
        [HttpGet("recurso/{recursoId}")]
        public async Task<ActionResult<IEnumerable<AsignacionDeRecursoDto>>> GetAssignmentsByResource(int recursoId)
        {
            try
            {
                var assignments = await _assignmentService.GetByResourceAsync(recursoId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener asignaciones del recurso", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener asignaciones pendientes de confirmación
        /// </summary>
        [HttpGet("pendientes")]
        public async Task<ActionResult<IEnumerable<AsignacionDeRecursoDto>>> GetPendingAssignments()
        {
            try
            {
                var assignments = await _assignmentService.GetPendingAssignmentsAsync();
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener asignaciones pendientes", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener asignaciones con devolución vencida
        /// </summary>
        [HttpGet("vencidas")]
        public async Task<ActionResult<IEnumerable<AsignacionDeRecursoDto>>> GetOverdueReturns()
        {
            try
            {
                var assignments = await _assignmentService.GetOverdueReturnsAsync();
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener devoluciones vencidas", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener asignaciones activas
        /// </summary>
        [HttpGet("activas")]
        public async Task<ActionResult<IEnumerable<AsignacionDeRecursoDto>>> GetActiveAssignments()
        {
            try
            {
                var assignments = await _assignmentService.GetActiveAssignmentsAsync();
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener asignaciones activas", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener mis asignaciones (del usuario autenticado)
        /// </summary>
        [HttpGet("mis-asignaciones")]
        public async Task<ActionResult<IEnumerable<AsignacionDeRecursoDto>>> GetMyAssignments()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var assignments = await _assignmentService.GetByVolunteerAsync(currentUserId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener mis asignaciones", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener estadísticas de asignaciones
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<object>> GetAssignmentStatistics()
        {
            try
            {
                var statistics = await _assignmentService.GetAssignmentStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener estadísticas", error = ex.Message });
            }
        }
    }
}
