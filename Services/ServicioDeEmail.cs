using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace ApiPG.Services
{
    public interface IServicioDeEmail
    {
        Task<bool> EnviarEmailRecuperacionAsync(string emailDestino, string nombreUsuario, string token, DateTime fechaExpiracion);
        Task<bool> EnviarEmailAsync(string emailDestino, string asunto, string cuerpoHtml);
    }

    public class ServicioDeEmail : IServicioDeEmail
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServicioDeEmail> _logger;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly string _senderPassword;
        private readonly bool _enableSsl;

        public ServicioDeEmail(IConfiguration configuration, ILogger<ServicioDeEmail> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var emailSettings = _configuration.GetSection("EmailSettings");
            _smtpServer = emailSettings["SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
            _senderEmail = emailSettings["SenderEmail"] ?? "";
            _senderName = emailSettings["SenderName"] ?? "API PG";
            _senderPassword = emailSettings["SenderPassword"] ?? "";
            _enableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "true");
        }

        public async Task<bool> EnviarEmailRecuperacionAsync(
            string emailDestino, 
            string nombreUsuario, 
            string token, 
            DateTime fechaExpiracion)
        {
            var asunto = "Recuperación de Contraseña - API PG";
            
            var cuerpoHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .token-box {{ background-color: #fff; border: 2px solid #4CAF50; padding: 15px; margin: 20px 0; text-align: center; font-size: 18px; font-weight: bold; letter-spacing: 2px; }}
        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #777; }}
        .warning {{ color: #f44336; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 Recuperación de Contraseña</h1>
        </div>
        <div class='content'>
            <p>Hola <strong>{nombreUsuario}</strong>,</p>
            
            <p>Recibimos una solicitud para restablecer tu contraseña en <strong>API PG</strong>.</p>
            
            <p>Usa el siguiente código para crear una nueva contraseña:</p>
            
            <div class='token-box'>
                {token}
            </div>
            
            <p><strong>⏰ Este código expirará el:</strong> {fechaExpiracion:dd/MM/yyyy HH:mm} (UTC)</p>
            
            <p><strong>Tiempo restante:</strong> 24 horas</p>
            
            <hr>
            
            <p class='warning'>⚠️ Si NO solicitaste este cambio:</p>
            <ul>
                <li>Ignora este correo electrónico</li>
                <li>Tu contraseña actual permanecerá sin cambios</li>
                <li>Considera cambiar tu contraseña por seguridad</li>
            </ul>
            
            <p><strong>Consejos de seguridad:</strong></p>
            <ul>
                <li>Nunca compartas este código con nadie</li>
                <li>Usa una contraseña fuerte y única</li>
                <li>No uses la misma contraseña en múltiples sitios</li>
            </ul>
        </div>
        <div class='footer'>
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2025 API PG. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

            return await EnviarEmailAsync(emailDestino, asunto, cuerpoHtml);
        }

        public async Task<bool> EnviarEmailAsync(string emailDestino, string asunto, string cuerpoHtml)
        {
            // Validar que la configuración esté completa
            if (string.IsNullOrEmpty(_senderEmail) || string.IsNullOrEmpty(_senderPassword))
            {
                _logger.LogWarning("Configuración de email incompleta. No se puede enviar correo.");
                return false;
            }

            try
            {
                using var smtpClient = new SmtpClient(_smtpServer, _smtpPort)
                {
                    EnableSsl = _enableSsl,
                    Credentials = new NetworkCredential(_senderEmail, _senderPassword),
                    Timeout = 10000 // 10 segundos
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail, _senderName),
                    Subject = asunto,
                    Body = cuerpoHtml,
                    IsBodyHtml = true,
                    Priority = MailPriority.High
                };

                mailMessage.To.Add(emailDestino);

                await smtpClient.SendMailAsync(mailMessage);
                
                _logger.LogInformation($"Email enviado exitosamente a {emailDestino}");
                return true;
            }
            catch (SmtpException ex)
            {
                _logger.LogError($"Error SMTP al enviar email: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar email: {ex.Message}");
                return false;
            }
        }
    }
}
