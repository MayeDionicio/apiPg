using ApiPG.Data;
using ApiPG.DTOs;
using ApiPG.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ApiPG.Services
{
    public interface IServicioDeRecuperacion
    {
        Task<RespuestaRecuperacionDto> SolicitarRecuperacionAsync(SolicitarRecuperacionDto dto);
        Task<RespuestaRecuperacionDto> ValidarTokenAsync(ValidarTokenDto dto);
        Task<RespuestaRecuperacionDto> RestablecerContrasenaAsync(RestablecerContrasenaDto dto);
        Task LimpiarTokensExpiradosAsync();
    }

    public class ServicioDeRecuperacion : IServicioDeRecuperacion
    {
        private readonly ApiPGContext _context;
        private readonly IServicioDeEmail _servicioEmail;
        private readonly ILogger<ServicioDeRecuperacion> _logger;
        private readonly IConfiguration _configuration;
        private const int DURACION_TOKEN_HORAS = 24; // Token válido por 24 horas

        public ServicioDeRecuperacion(
            ApiPGContext context, 
            IServicioDeEmail servicioEmail,
            ILogger<ServicioDeRecuperacion> logger,
            IConfiguration configuration)
        {
            _context = context;
            _servicioEmail = servicioEmail;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<RespuestaRecuperacionDto> SolicitarRecuperacionAsync(SolicitarRecuperacionDto dto)
        {
            // Buscar usuario por correo
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoElectronico == dto.Email && u.EstaActivo);

            // Por seguridad, siempre retornar mensaje exitoso aunque el usuario no exista
            // Esto previene que se pueda verificar qué correos están registrados
            if (usuario == null)
            {
                return new RespuestaRecuperacionDto
                {
                    Exitoso = true,
                    Mensaje = "Si el correo existe, recibirás un enlace de recuperación"
                };
            }

            // Generar token único
            var token = GenerarToken();
            var fechaExpiracion = DateTime.UtcNow.AddHours(DURACION_TOKEN_HORAS);

            // Invalidar tokens anteriores del usuario que no hayan sido usados
            var tokensAnteriores = await _context.TokensRecuperacion
                .Where(t => t.IdUsuario == usuario.Id && !t.Usado && t.FechaExpiracion > DateTime.UtcNow)
                .ToListAsync();

            foreach (var tokenAnterior in tokensAnteriores)
            {
                tokenAnterior.Usado = true;
            }

            // Crear nuevo token
            var tokenRecuperacion = new TokenRecuperacion
            {
                IdUsuario = usuario.Id,
                Token = token,
                FechaExpiracion = fechaExpiracion,
                Usado = false,
                CreadoEn = DateTime.UtcNow
            };

            _context.TokensRecuperacion.Add(tokenRecuperacion);
            await _context.SaveChangesAsync();

            // Verificar si el envío de emails está habilitado
            var emailSettings = _configuration.GetSection("EmailSettings");
            var emailHabilitado = !string.IsNullOrEmpty(emailSettings["SenderEmail"]) && 
                                  !string.IsNullOrEmpty(emailSettings["SenderPassword"]);

            if (emailHabilitado)
            {
                // MODO PRODUCCIÓN: Enviar email y NO retornar el token
                var emailEnviado = await _servicioEmail.EnviarEmailRecuperacionAsync(
                    usuario.CorreoElectronico,
                    usuario.PrimerNombre,
                    token,
                    fechaExpiracion
                );

                if (emailEnviado)
                {
                    _logger.LogInformation($"Email de recuperación enviado a {usuario.CorreoElectronico}");
                    return new RespuestaRecuperacionDto
                    {
                        Exitoso = true,
                        Mensaje = "Se ha enviado un correo con las instrucciones de recuperación"
                    };
                }
                else
                {
                    _logger.LogWarning($"No se pudo enviar email a {usuario.CorreoElectronico}");
                    return new RespuestaRecuperacionDto
                    {
                        Exitoso = false,
                        Mensaje = "Error al enviar el correo de recuperación. Intenta nuevamente."
                    };
                }
            }
            else
            {
                // MODO DESARROLLO: Retornar el token directamente
                _logger.LogWarning("Email no configurado. Retornando token en respuesta (solo desarrollo)");
                return new RespuestaRecuperacionDto
                {
                    Exitoso = true,
                    Mensaje = "Token generado exitosamente. En producción se enviaría por email.",
                    Token = token, // Solo para desarrollo
                    FechaExpiracion = fechaExpiracion
                };
            }
        }

        public async Task<RespuestaRecuperacionDto> ValidarTokenAsync(ValidarTokenDto dto)
        {
            var tokenRecuperacion = await _context.TokensRecuperacion
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.Token == dto.Token);

            if (tokenRecuperacion == null)
            {
                return new RespuestaRecuperacionDto
                {
                    Exitoso = false,
                    Mensaje = "Token inválido"
                };
            }

            if (tokenRecuperacion.Usado)
            {
                return new RespuestaRecuperacionDto
                {
                    Exitoso = false,
                    Mensaje = "Este token ya fue utilizado"
                };
            }

            if (tokenRecuperacion.FechaExpiracion < DateTime.UtcNow)
            {
                return new RespuestaRecuperacionDto
                {
                    Exitoso = false,
                    Mensaje = "El token ha expirado"
                };
            }

            if (!tokenRecuperacion.Usuario.EstaActivo)
            {
                return new RespuestaRecuperacionDto
                {
                    Exitoso = false,
                    Mensaje = "Usuario inactivo"
                };
            }

            return new RespuestaRecuperacionDto
            {
                Exitoso = true,
                Mensaje = "Token válido",
                FechaExpiracion = tokenRecuperacion.FechaExpiracion
            };
        }

        public async Task<RespuestaRecuperacionDto> RestablecerContrasenaAsync(RestablecerContrasenaDto dto)
        {
            // Validar token
            var validacion = await ValidarTokenAsync(new ValidarTokenDto { Token = dto.Token });
            if (!validacion.Exitoso)
            {
                return validacion;
            }

            // Obtener token y usuario
            var tokenRecuperacion = await _context.TokensRecuperacion
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.Token == dto.Token);

            if (tokenRecuperacion == null)
            {
                return new RespuestaRecuperacionDto
                {
                    Exitoso = false,
                    Mensaje = "Token no encontrado"
                };
            }

            // Actualizar contraseña
            var usuario = tokenRecuperacion.Usuario;
            usuario.HashDeContrasena = HashPassword(dto.NuevaContrasena);
            usuario.ActualizadoEn = DateTime.UtcNow;

            // Marcar token como usado
            tokenRecuperacion.Usado = true;

            await _context.SaveChangesAsync();

            return new RespuestaRecuperacionDto
            {
                Exitoso = true,
                Mensaje = "Contraseña restablecida exitosamente"
            };
        }

        public async Task LimpiarTokensExpiradosAsync()
        {
            // Eliminar tokens que expiraron hace más de 7 días
            var fechaLimite = DateTime.UtcNow.AddDays(-7);
            
            var tokensExpirados = await _context.TokensRecuperacion
                .Where(t => t.FechaExpiracion < fechaLimite)
                .ToListAsync();

            if (tokensExpirados.Any())
            {
                _context.TokensRecuperacion.RemoveRange(tokensExpirados);
                await _context.SaveChangesAsync();
            }
        }

        private static string GenerarToken()
        {
            // Generar un token seguro de 32 caracteres
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "")
                .Substring(0, 32);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "ApiPG_Salt"));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
