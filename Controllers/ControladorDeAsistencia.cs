using ApiPG.DTOs;
using ApiPG.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/asistencia")]
    public class AsistenciaController : ControllerBase
    {
        private readonly IAttendanceService _service;

        public AsistenciaController(IAttendanceService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] CrearAsistenciaDto dto)
        {
            var added = await _service.AddAsync(dto);
            if (added == null) return BadRequest();
            return CreatedAtAction(nameof(GetByLevelAndDate), new { nivelId = added.NivelId, fecha = added.Fecha.ToString("yyyy-MM-dd") }, added);
        }

        [HttpGet("nivel/{nivelId}")]
        public async Task<IActionResult> GetByLevelAndDate(int nivelId, [FromQuery] DateTime fecha)
        {
            var list = await _service.GetByLevelAndDateAsync(nivelId, fecha);
            return Ok(list);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetByUser(int usuarioId, [FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null)
        {
            var list = await _service.GetByUserAsync(usuarioId, desde, hasta);
            return Ok(list);
        }

        [HttpGet("reporte")]
        [Authorize]
        public async Task<IActionResult> GetReport([FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null, [FromQuery] int? nivelId = null)
        {
            var list = await _service.GetReportAsync(desde, hasta, nivelId);
            return Ok(list);
        }
    }
}
