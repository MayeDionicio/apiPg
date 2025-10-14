using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiPG.Services;
using ApiPG.DTOs;
using System.Security.Claims;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/recursos")]
    [Authorize]
    public class RecursosController : ControllerBase
    {
        private readonly IResourceService _resourceService;

        public RecursosController(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        /// <summary>
        /// Obtener todos los recursos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecursoDto>>> GetResources([FromQuery] bool soloActivos = false)
        {
            try
            {
                var resources = soloActivos 
                    ? await _resourceService.GetActiveAsync()
                    : await _resourceService.GetAllAsync();
                
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los recursos", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener un recurso por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RecursoDto>> GetResource(int id)
        {
            try
            {
                var resource = await _resourceService.GetByIdAsync(id);
                if (resource == null)
                {
                    return NotFound(new { message = $"Recurso con ID {id} no encontrado" });
                }
                return Ok(resource);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el recurso", error = ex.Message });
            }
        }

        /// <summary>
        /// Crear un nuevo recurso
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<RecursoDto>> CreateResource([FromBody] CrearRecursoDto createResourceDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resource = await _resourceService.CreateAsync(createResourceDto);
                return CreatedAtAction(nameof(GetResource), new { id = resource.Id }, resource);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el recurso", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar un recurso existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<RecursoDto>> UpdateResource(int id, [FromBody] ActualizarRecursoDto updateResourceDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resource = await _resourceService.UpdateAsync(id, updateResourceDto);
                if (resource == null)
                {
                    return NotFound(new { message = $"Recurso con ID {id} no encontrado" });
                }

                return Ok(resource);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el recurso", error = ex.Message });
            }
        }

        /// <summary>
        /// Desactivar un recurso (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResource(int id)
        {
            try
            {
                var result = await _resourceService.SoftDeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Recurso con ID {id} no encontrado" });
                }

                return Ok(new { message = "Recurso desactivado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al desactivar el recurso", error = ex.Message });
            }
        }

        /// <summary>
        /// Activar un recurso
        /// </summary>
        [HttpPut("{id}/activar")]
        public async Task<IActionResult> ActivateResource(int id)
        {
            try
            {
                var result = await _resourceService.ActivateAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Recurso con ID {id} no encontrado" });
                }

                return Ok(new { message = "Recurso activado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al activar el recurso", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener recursos por categoría
        /// </summary>
        [HttpGet("categoria/{categoria}")]
        public async Task<ActionResult<IEnumerable<RecursoDto>>> GetResourcesByCategory(string categoria)
        {
            try
            {
                var resources = await _resourceService.GetByCategoryAsync(categoria);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener recursos por categoría", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener recursos disponibles (con cantidad > 0)
        /// </summary>
        [HttpGet("disponibles")]
        public async Task<ActionResult<IEnumerable<RecursoDto>>> GetAvailableResources()
        {
            try
            {
                var resources = await _resourceService.GetAvailableAsync();
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener recursos disponibles", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener recursos con stock bajo
        /// </summary>
        [HttpGet("stock-bajo")]
        public async Task<ActionResult<IEnumerable<RecursoDto>>> GetLowStockResources([FromQuery] int threshold = 5)
        {
            try
            {
                var resources = await _resourceService.GetLowStockAsync(threshold);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener recursos con stock bajo", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar cantidad de un recurso
        /// </summary>
        [HttpPut("{id}/cantidad")]
        public async Task<IActionResult> UpdateResourceQuantity(int id, [FromBody] ActualizarCantidadDto ActualizarCantidadDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _resourceService.UpdateQuantityAsync(id, ActualizarCantidadDto.NuevaCantidad);
                if (!result)
                {
                    return NotFound(new { message = $"Recurso con ID {id} no encontrado" });
                }

                return Ok(new { message = "Cantidad actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la cantidad", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener estadísticas de recursos
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<object>> GetResourceStatistics()
        {
            try
            {
                var statistics = await _resourceService.GetResourceStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener estadísticas", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener todas las categorías disponibles
        /// </summary>
        [HttpGet("categorias")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            try
            {
                var categories = await _resourceService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener categorías", error = ex.Message });
            }
        }
    }

    // DTO adicional para actualizar cantidad
    public class ActualizarCantidadDto
    {
        public int NuevaCantidad { get; set; }
    }
}
