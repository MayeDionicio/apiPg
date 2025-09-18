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
    public class ResourceUsageLogsController : ControllerBase
    {
        private readonly IResourceUsageLogService _usageLogService;

        public ResourceUsageLogsController(IResourceUsageLogService usageLogService)
        {
            _usageLogService = usageLogService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        /// <summary>
        /// Obtener todos los registros de uso
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceUsageLogDto>>> GetUsageLogs()
        {
            try
            {
                var logs = await _usageLogService.GetAllAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros de uso", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener un registro de uso por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceUsageLogDto>> GetUsageLog(int id)
        {
            try
            {
                var log = await _usageLogService.GetByIdAsync(id);
                if (log == null)
                {
                    return NotFound(new { message = $"Registro de uso con ID {id} no encontrado" });
                }
                return Ok(log);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el registro de uso", error = ex.Message });
            }
        }

        /// <summary>
        /// Crear un nuevo registro de uso/incidente
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ResourceUsageLogDto>> CreateUsageLog([FromBody] CreateResourceUsageLogDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = GetCurrentUserId();
                var log = await _usageLogService.CreateAsync(createDto, currentUserId);
                return CreatedAtAction(nameof(GetUsageLog), new { id = log.Id }, log);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el registro de uso", error = ex.Message });
            }
        }

        /// <summary>
        /// Resolver un incidente
        /// </summary>
        [HttpPut("{id}/resolve")]
        public async Task<ActionResult<ResourceUsageLogDto>> ResolveIncident(int id, [FromBody] ResolveIncidentDto resolveDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = GetCurrentUserId();
                var log = await _usageLogService.ResolveIncidentAsync(id, currentUserId, resolveDto);
                if (log == null)
                {
                    return NotFound(new { message = $"Registro de uso con ID {id} no encontrado" });
                }

                return Ok(log);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al resolver el incidente", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener registros de uso por asignación
        /// </summary>
        [HttpGet("assignment/{assignmentId}")]
        public async Task<ActionResult<IEnumerable<ResourceUsageLogDto>>> GetUsageLogsByAssignment(int assignmentId)
        {
            try
            {
                var logs = await _usageLogService.GetByAssignmentAsync(assignmentId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener registros por asignación", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener registros de uso por recurso
        /// </summary>
        [HttpGet("resource/{resourceId}")]
        public async Task<ActionResult<IEnumerable<ResourceUsageLogDto>>> GetUsageLogsByResource(int resourceId)
        {
            try
            {
                var logs = await _usageLogService.GetByResourceAsync(resourceId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener registros por recurso", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener registros de uso por voluntario
        /// </summary>
        [HttpGet("volunteer/{volunteerId}")]
        public async Task<ActionResult<IEnumerable<ResourceUsageLogDto>>> GetUsageLogsByVolunteer(int volunteerId)
        {
            try
            {
                var logs = await _usageLogService.GetByVolunteerAsync(volunteerId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener registros por voluntario", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener todos los incidentes
        /// </summary>
        [HttpGet("incidents")]
        public async Task<ActionResult<IEnumerable<ResourceUsageLogDto>>> GetIncidents()
        {
            try
            {
                var incidents = await _usageLogService.GetIncidentsAsync();
                return Ok(incidents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener incidentes", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener incidentes sin resolver
        /// </summary>
        [HttpGet("incidents/unresolved")]
        public async Task<ActionResult<IEnumerable<ResourceUsageLogDto>>> GetUnresolvedIncidents()
        {
            try
            {
                var incidents = await _usageLogService.GetUnresolvedIncidentsAsync();
                return Ok(incidents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener incidentes sin resolver", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener registros por tipo de evento
        /// </summary>
        [HttpGet("event-type/{eventType}")]
        public async Task<ActionResult<IEnumerable<ResourceUsageLogDto>>> GetUsageLogsByEventType(UsageEventType eventType)
        {
            try
            {
                var logs = await _usageLogService.GetByEventTypeAsync(eventType);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener registros por tipo de evento", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener actividades recientes
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<ResourceUsageLogDto>>> GetRecentActivities([FromQuery] int take = 20)
        {
            try
            {
                var logs = await _usageLogService.GetRecentActivitiesAsync(take);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener actividades recientes", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener estadísticas de uso
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetUsageStatistics()
        {
            try
            {
                var statistics = await _usageLogService.GetUsageStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener estadísticas", error = ex.Message });
            }
        }

        /// <summary>
        /// Confirmar uso de recurso (acción común del voluntario)
        /// </summary>
        [HttpPost("confirm-usage")]
        public async Task<ActionResult<ResourceUsageLogDto>> ConfirmUsage([FromBody] ConfirmUsageDto confirmDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createDto = new CreateResourceUsageLogDto
                {
                    ResourceAssignmentId = confirmDto.ResourceAssignmentId,
                    EventType = UsageEventType.ConfirmationOfUse,
                    Title = "Confirmación de uso del recurso",
                    Description = confirmDto.Description ?? "El voluntario ha confirmado el uso del recurso",
                    ConditionBefore = confirmDto.ConditionBefore,
                    ConditionAfter = confirmDto.ConditionAfter,
                    ActionsTaken = confirmDto.ActionsTaken,
                    PhotoUrls = confirmDto.PhotoUrls
                };

                var currentUserId = GetCurrentUserId();
                var log = await _usageLogService.CreateAsync(createDto, currentUserId);
                return CreatedAtAction(nameof(GetUsageLog), new { id = log.Id }, log);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al confirmar el uso", error = ex.Message });
            }
        }

        /// <summary>
        /// Reportar incidente rápido
        /// </summary>
        [HttpPost("report-incident")]
        public async Task<ActionResult<ResourceUsageLogDto>> ReportIncident([FromBody] ReportIncidentDto incidentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createDto = new CreateResourceUsageLogDto
                {
                    ResourceAssignmentId = incidentDto.ResourceAssignmentId,
                    EventType = UsageEventType.IncidentReport,
                    Title = incidentDto.Title,
                    Description = incidentDto.Description,
                    ConditionBefore = incidentDto.ConditionBefore,
                    ConditionAfter = incidentDto.ConditionAfter,
                    QuantityAffected = incidentDto.QuantityAffected,
                    ActionsTaken = incidentDto.ActionsTaken,
                    Recommendations = incidentDto.Recommendations,
                    RequiresFollowUp = true,
                    PhotoUrls = incidentDto.PhotoUrls
                };

                var currentUserId = GetCurrentUserId();
                var log = await _usageLogService.CreateAsync(createDto, currentUserId);
                return CreatedAtAction(nameof(GetUsageLog), new { id = log.Id }, log);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al reportar el incidente", error = ex.Message });
            }
        }
    }

    // DTOs adicionales para endpoints específicos
    public class ConfirmUsageDto
    {
        public int ResourceAssignmentId { get; set; }
        public string? Description { get; set; }
        public ResourceCondition? ConditionBefore { get; set; }
        public ResourceCondition? ConditionAfter { get; set; }
        public string? ActionsTaken { get; set; }
        public string? PhotoUrls { get; set; }
    }

    public class ReportIncidentDto
    {
        public int ResourceAssignmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ResourceCondition? ConditionBefore { get; set; }
        public ResourceCondition? ConditionAfter { get; set; }
        public int? QuantityAffected { get; set; }
        public string? ActionsTaken { get; set; }
        public string? Recommendations { get; set; }
        public string? PhotoUrls { get; set; }
    }
}