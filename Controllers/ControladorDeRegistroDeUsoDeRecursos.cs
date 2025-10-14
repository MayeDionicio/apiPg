using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiPG.Services;
using ApiPG.DTOs;
using ApiPG.Models;
using System.Security.Claims;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/registros-uso-recursos")]
    [Authorize]
    public class RegistroUsoRecursosController : ControllerBase
    {
        private readonly IResourceUsageLogService _usageLogService;

        public RegistroUsoRecursosController(IResourceUsageLogService usageLogService)
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
        public async Task<ActionResult<IEnumerable<RegistroDeUsoDeRecursoDto>>> GetUsageLogs()
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
        public async Task<ActionResult<RegistroDeUsoDeRecursoDto>> GetUsageLog(int id)
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
        public async Task<ActionResult<RegistroDeUsoDeRecursoDto>> CreateUsageLog([FromBody] CrearRegistroDeUsoDeRecursoDto createDto)
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
        [HttpPut("{id}/resolver")]
        public async Task<ActionResult<RegistroDeUsoDeRecursoDto>> ResolveIncident(int id, [FromBody] ResolverIncidenteDto resolveDto)
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
        [HttpGet("asignacion/{asignacionId}")]
        public async Task<ActionResult<IEnumerable<RegistroDeUsoDeRecursoDto>>> GetUsageLogsByAssignment(int asignacionId)
        {
            try
            {
                var logs = await _usageLogService.GetByAssignmentAsync(asignacionId);
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
        [HttpGet("recurso/{recursoId}")]
        public async Task<ActionResult<IEnumerable<RegistroDeUsoDeRecursoDto>>> GetUsageLogsByResource(int recursoId)
        {
            try
            {
                var logs = await _usageLogService.GetByResourceAsync(recursoId);
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
        [HttpGet("voluntario/{voluntarioId}")]
        public async Task<ActionResult<IEnumerable<RegistroDeUsoDeRecursoDto>>> GetUsageLogsByVolunteer(int voluntarioId)
        {
            try
            {
                var logs = await _usageLogService.GetByVolunteerAsync(voluntarioId);
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
        [HttpGet("incidentes")]
        public async Task<ActionResult<IEnumerable<RegistroDeUsoDeRecursoDto>>> GetIncidents()
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
        [HttpGet("incidentes/sin-resolver")]
        public async Task<ActionResult<IEnumerable<RegistroDeUsoDeRecursoDto>>> GetUnresolvedIncidents()
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
        [HttpGet("tipo-evento/{tipoEvento}")]
        public async Task<ActionResult<IEnumerable<RegistroDeUsoDeRecursoDto>>> GetUsageLogsByEventType(TipoDeEventoDeUso tipoEvento)
        {
            try
            {
                var logs = await _usageLogService.GetByEventTypeAsync(tipoEvento);
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
        [HttpGet("recientes")]
        public async Task<ActionResult<IEnumerable<RegistroDeUsoDeRecursoDto>>> GetRecentActivities([FromQuery] int take = 20)
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
        [HttpGet("estadisticas")]
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
        [HttpPost("confirmar-uso")]
        public async Task<ActionResult<RegistroDeUsoDeRecursoDto>> ConfirmUsage([FromBody] ConfirmarUsoDto confirmDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createDto = new CrearRegistroDeUsoDeRecursoDto
                {
                    AsignacionDeRecursoId = confirmDto.AsignacionDeRecursoId,
                    TipoDeEvento = TipoDeEventoDeUso.ConfirmacionDeUso,
                    Titulo = "Confirmación de uso del recurso",
                    Descripcion = confirmDto.Descripcion ?? "El voluntario ha confirmado el uso del recurso",
                    CondicionAntes = confirmDto.CondicionAntes,
                    CondicionDespues = confirmDto.CondicionDespues,
                    AccionesTomadas = confirmDto.AccionesTomadas,
                    UrlsDeFotos = confirmDto.UrlsDeFotos
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
        [HttpPost("reportar-incidente")]
        public async Task<ActionResult<RegistroDeUsoDeRecursoDto>> ReportIncident([FromBody] ReportIncidentDto incidentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createDto = new CrearRegistroDeUsoDeRecursoDto
                {
                    AsignacionDeRecursoId = incidentDto.AsignacionDeRecursoId,
                    TipoDeEvento = TipoDeEventoDeUso.ReporteDeIncidente,
                    Titulo = incidentDto.Titulo,
                    Descripcion = incidentDto.Descripcion,
                    CondicionAntes = incidentDto.CondicionAntes,
                    CondicionDespues = incidentDto.CondicionDespues,
                    CantidadAfectada = incidentDto.CantidadAfectada,
                    AccionesTomadas = incidentDto.AccionesTomadas,
                    Recomendaciones = incidentDto.Recomendaciones,
                    RequiereSeguimiento = true,
                    UrlsDeFotos = incidentDto.UrlsDeFotos
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
    public class ConfirmarUsoDto
    {
        public int AsignacionDeRecursoId { get; set; }
        public string? Descripcion { get; set; }
        public CondicionDelRecurso? CondicionAntes { get; set; }
        public CondicionDelRecurso? CondicionDespues { get; set; }
        public string? AccionesTomadas { get; set; }
        public string? UrlsDeFotos { get; set; }
    }

    public class ReportIncidentDto
    {
        public int AsignacionDeRecursoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public CondicionDelRecurso? CondicionAntes { get; set; }
        public CondicionDelRecurso? CondicionDespues { get; set; }
        public int? CantidadAfectada { get; set; }
        public string? AccionesTomadas { get; set; }
        public string? Recomendaciones { get; set; }
        public string? UrlsDeFotos { get; set; }
    }
}
