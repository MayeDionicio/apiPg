using ApiPG.DTOs;
using ApiPG.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControladorDeRecuperacion : ControllerBase
    {
        private readonly IServicioDeRecuperacion _servicio;

        public ControladorDeRecuperacion(IServicioDeRecuperacion servicio)
        {
            _servicio = servicio;
        }

        /// <summary>
        /// Solicita un token de recuperación de contraseña
        /// Envía un correo electrónico con el token (en desarrollo retorna el token)
        /// </summary>
        [HttpPost("solicitar")]
        public async Task<ActionResult<RespuestaRecuperacionDto>> SolicitarRecuperacion(
            [FromBody] SolicitarRecuperacionDto dto)
        {
            try
            {
                var resultado = await _servicio.SolicitarRecuperacionAsync(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaRecuperacionDto
                {
                    Exitoso = false,
                    Mensaje = $"Error al procesar la solicitud: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Valida si un token de recuperación es válido y no ha expirado
        /// </summary>
        [HttpPost("validar-token")]
        public async Task<ActionResult<RespuestaRecuperacionDto>> ValidarToken(
            [FromBody] ValidarTokenDto dto)
        {
            try
            {
                var resultado = await _servicio.ValidarTokenAsync(dto);
                
                if (!resultado.Exitoso)
                {
                    return BadRequest(resultado);
                }
                
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaRecuperacionDto
                {
                    Exitoso = false,
                    Mensaje = $"Error al validar el token: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Restablece la contraseña usando un token válido
        /// </summary>
        [HttpPost("restablecer")]
        public async Task<ActionResult<RespuestaRecuperacionDto>> RestablecerContrasena(
            [FromBody] RestablecerContrasenaDto dto)
        {
            try
            {
                var resultado = await _servicio.RestablecerContrasenaAsync(dto);
                
                if (!resultado.Exitoso)
                {
                    return BadRequest(resultado);
                }
                
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaRecuperacionDto
                {
                    Exitoso = false,
                    Mensaje = $"Error al restablecer la contraseña: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Limpia tokens expirados (debe ser llamado periódicamente por un job)
        /// </summary>
        [HttpPost("limpiar-tokens")]
        public async Task<ActionResult> LimpiarTokensExpirados()
        {
            try
            {
                await _servicio.LimpiarTokensExpiradosAsync();
                return Ok(new { mensaje = "Tokens expirados eliminados exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al limpiar tokens: {ex.Message}" });
            }
        }
    }
}
