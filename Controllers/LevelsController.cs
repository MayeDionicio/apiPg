using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiPG.Services;
using ApiPG.DTOs;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LevelsController : ControllerBase
    {
        private readonly ILevelService _levelService;

        public LevelsController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        /// <summary>
        /// Obtener todos los niveles
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LevelDto>>> GetLevels()
        {
            try
            {
                var levels = await _levelService.GetAllAsync();
                return Ok(levels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los niveles", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener un nivel por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LevelDto>> GetLevel(int id)
        {
            try
            {
                var level = await _levelService.GetByIdAsync(id);
                if (level == null)
                {
                    return NotFound(new { message = $"Nivel con ID {id} no encontrado" });
                }
                return Ok(level);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el nivel", error = ex.Message });
            }
        }

        /// <summary>
        /// Crear un nuevo nivel
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<LevelDto>> CreateLevel([FromBody] CreateLevelDto createLevelDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var level = await _levelService.CreateAsync(createLevelDto);
                return CreatedAtAction(nameof(GetLevel), new { id = level.Id }, level);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el nivel", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar un nivel existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<LevelDto>> UpdateLevel(int id, [FromBody] UpdateLevelDto updateLevelDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var level = await _levelService.UpdateAsync(id, updateLevelDto);
                if (level == null)
                {
                    return NotFound(new { message = $"Nivel con ID {id} no encontrado" });
                }

                return Ok(level);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el nivel", error = ex.Message });
            }
        }

        /// <summary>
        /// Eliminar un nivel (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLevel(int id)
        {
            try
            {
                var result = await _levelService.SoftDeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Nivel con ID {id} no encontrado" });
                }

                return Ok(new { message = "Nivel eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el nivel", error = ex.Message });
            }
        }

        /// <summary>
        /// Asignar tutor a un nivel
        /// </summary>
        [HttpPut("{levelId}/assign-tutor")]
        public async Task<ActionResult<LevelDto>> AssignTutor(int levelId, [FromBody] AssignTutorDto assignTutorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _levelService.ChangeTutorAsync(levelId, assignTutorDto.TutorId);
                if (!result)
                {
                    return NotFound(new { message = $"Nivel con ID {levelId} no encontrado" });
                }

                var updatedLevel = await _levelService.GetByIdAsync(levelId);
                return Ok(updatedLevel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al asignar tutor al nivel", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener participantes de un nivel específico
        /// </summary>
        [HttpGet("{levelId}/participants")]
        public async Task<ActionResult<IEnumerable<LevelParticipantDto>>> GetLevelParticipants(int levelId)
        {
            try
            {
                var level = await _levelService.GetByIdAsync(levelId);
                if (level == null)
                {
                    return NotFound(new { message = $"Nivel con ID {levelId} no encontrado" });
                }

                var participants = await _levelService.GetParticipantsAsync(levelId);
                return Ok(participants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener participantes del nivel", error = ex.Message });
            }
        }

        /// <summary>
        /// Asignar un participante a un nivel
        /// </summary>
        [HttpPost("{levelId}/participants/{userId}")]
        public async Task<IActionResult> AssignParticipantToLevel(int levelId, int userId)
        {
            try
            {
                var levelExists = await _levelService.GetByIdAsync(levelId);
                if (levelExists == null)
                {
                    return NotFound(new { message = $"Nivel con ID {levelId} no encontrado" });
                }

                var result = await _levelService.AssignParticipantAsync(levelId, userId);
                if (!result)
                {
                    return BadRequest(new { message = "Error al asignar participante al nivel" });
                }

                return Ok(new { message = "Participante asignado al nivel correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al asignar participante al nivel", error = ex.Message });
            }
        }

        /// <summary>
        /// Remover un participante de un nivel
        /// </summary>
        [HttpDelete("{levelId}/participants/{userId}")]
        public async Task<IActionResult> RemoveParticipantFromLevel(int levelId, int userId)
        {
            try
            {
                var result = await _levelService.RemoveParticipantAsync(levelId, userId);
                if (!result)
                {
                    return NotFound(new { message = "Participante no encontrado en el nivel o ya removido" });
                }

                return Ok(new { message = "Participante removido del nivel correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al remover participante del nivel", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener niveles asignados a un tutor específico
        /// </summary>
        [HttpGet("by-tutor/{tutorId}")]
        public async Task<ActionResult<IEnumerable<LevelDto>>> GetLevelsByTutor(int tutorId)
        {
            try
            {
                var levels = await _levelService.GetLevelsByTutorAsync(tutorId);
                return Ok(levels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener niveles del tutor", error = ex.Message });
            }
        }
    }

    // DTO adicional para asignación de tutor
    public class AssignTutorDto
    {
        public int? TutorId { get; set; }
    }
}
