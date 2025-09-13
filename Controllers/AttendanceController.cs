using ApiPG.DTOs;
using ApiPG.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _service;

        public AttendanceController(IAttendanceService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] CreateAttendanceDto dto)
        {
            var added = await _service.AddAsync(dto);
            if (added == null) return BadRequest();
            return CreatedAtAction(nameof(GetByLevelAndDate), new { levelId = added.LevelId, date = added.Date.ToString("yyyy-MM-dd") }, added);
        }

        [HttpGet("level/{levelId}")]
        public async Task<IActionResult> GetByLevelAndDate(int levelId, [FromQuery] DateTime date)
        {
            var list = await _service.GetByLevelAndDateAsync(levelId, date);
            return Ok(list);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var list = await _service.GetByUserAsync(userId, from, to);
            return Ok(list);
        }

        [HttpGet("report")]
        [Authorize]
        public async Task<IActionResult> GetReport([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null, [FromQuery] int? levelId = null)
        {
            var list = await _service.GetReportAsync(from, to, levelId);
            return Ok(list);
        }
    }
}
