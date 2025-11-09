using System.ComponentModel.DataAnnotations;

namespace ApiPG.DTOs
{
    public class IniciarSesionDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Contrasena { get; set; } = string.Empty;
    }

    public class RespuestaInicioSesionDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public UsuarioDto User { get; set; } = null!;
    }

    public class RegistrarDto
    {
        [Required]
        [StringLength(100)]
        public string PrimerNombre { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string NombreDeUsuario { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Contrasena { get; set; } = string.Empty;
        
        [Required]
        public int RolId { get; set; } = 2; // Default to User role
        
        // Código del participante (opcional, solo para participantes)
        [StringLength(50)]
        public string? CodigoParticipante { get; set; }
        
        // Fecha de nacimiento (requerida si es participante)
        public DateTime? FechaDeNacimiento { get; set; }
    }

    public class RespuestaAutenticacionDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UsuarioDto? User { get; set; }
        public string? Token { get; set; }
    }
}
