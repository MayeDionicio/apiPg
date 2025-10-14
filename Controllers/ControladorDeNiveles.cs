using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiPG.Services;
using ApiPG.DTOs;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/niveles")]
    [Authorize]
    public class NivelesController : ControllerBase
    {
        private readonly ILevelService _levelService;

        public NivelesController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        /// <summary>
        /// Obtener todos los niveles
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NivelDto>>> GetLevels()
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
        public async Task<ActionResult<NivelDto>> GetLevel(int id)
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
        public async Task<ActionResult<NivelDto>> CreateLevel([FromBody] CrearNivelDto createLevelDto)
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
        public async Task<ActionResult<NivelDto>> UpdateLevel(int id, [FromBody] ActualizarNivelDto updateLevelDto)
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
        /// Asignar voluntario a un nivel
        /// </summary>
        [HttpPut("{nivelId}/asignar-voluntario")]
        public async Task<ActionResult<NivelDto>> AssignTutor(int nivelId, [FromBody] AsignarVoluntarioDto asignarVoluntarioDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _levelService.ChangeTutorAsync(nivelId, asignarVoluntarioDto.VoluntarioId);
                if (!result)
                {
                    return NotFound(new { message = $"Nivel con ID {nivelId} no encontrado" });
                }

                var updatedLevel = await _levelService.GetByIdAsync(nivelId);
                return Ok(updatedLevel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al asignar voluntario al nivel", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener participantes de un nivel específico
        /// </summary>
        [HttpGet("{nivelId}/participantes")]
        public async Task<ActionResult<IEnumerable<ParticipanteDeNivelDto>>> GetLevelParticipants(int nivelId)
        {
            try
            {
                var level = await _levelService.GetByIdAsync(nivelId);
                if (level == null)
                {
                    return NotFound(new { message = $"Nivel con ID {nivelId} no encontrado" });
                }

                var participants = await _levelService.GetParticipantsAsync(nivelId);
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
        [HttpPost("{nivelId}/participantes/{usuarioId}")]
        public async Task<IActionResult> AssignParticipantToLevel(int nivelId, int usuarioId)
        {
            try
            {
                var levelExists = await _levelService.GetByIdAsync(nivelId);
                if (levelExists == null)
                {
                    return NotFound(new { message = $"Nivel con ID {nivelId} no encontrado" });
                }

                var result = await _levelService.AssignParticipantAsync(nivelId, usuarioId);
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
        [HttpDelete("{nivelId}/participantes/{usuarioId}")]
        public async Task<IActionResult> RemoveParticipantFromLevel(int nivelId, int usuarioId)
        {
            try
            {
                var result = await _levelService.RemoveParticipantAsync(nivelId, usuarioId);
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
        /// Obtener niveles asignados a un voluntario específico
        /// </summary>
        [HttpGet("por-voluntario/{voluntarioId}")]
        public async Task<ActionResult<IEnumerable<NivelDto>>> GetLevelsByTutor(int voluntarioId)
        {
            try
            {
                var levels = await _levelService.GetLevelsByTutorAsync(voluntarioId);
                return Ok(levels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener niveles del voluntario", error = ex.Message });
            }
        }
    }

    // DTO adicional para asignación de tutor
    public class AsignarVoluntarioDto
    {
        public int? VoluntarioId { get; set; }
    }
}
