using System.ComponentModel.DataAnnotations;

namespace ApiPG.DTOs
{
    public class CrearUsuarioDto
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
        public int RolId { get; set; }
        
        // Código del participante (opcional, solo para participantes - RolId = 2)
        [StringLength(50)]
        public string? CodigoParticipante { get; set; }
        
        // Fecha de nacimiento (requerida solo para participantes - RolId = 2)
        public DateTime? FechaDeNacimiento { get; set; }
    }

    public class ActualizarUsuarioDto
    {
        [StringLength(100)]
        public string? PrimerNombre { get; set; }
        
        [StringLength(100)]
        public string? Apellido { get; set; }
        
        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }
        
        [StringLength(50)]
        public string? NombreDeUsuario { get; set; }
        
        [StringLength(100, MinimumLength = 6)]
        public string? Contrasena { get; set; }
        
        public int? RolId { get; set; }
        
        public bool? EstaActivo { get; set; }
        
        // Código del participante (opcional para actualización)
        [StringLength(50)]
        public string? CodigoParticipante { get; set; }
        
        // Fecha de nacimiento (opcional para actualización)
        public DateTime? FechaDeNacimiento { get; set; }
    }

    public class UsuarioDto
    {
        public int Id { get; set; }
        public string PrimerNombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreDeUsuario { get; set; } = string.Empty;
        public int RolId { get; set; }
        public string NombreRol { get; set; } = string.Empty;
        public DateTime CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public bool EstaActivo { get; set; }
        public string? CodigoParticipante { get; set; }
        public DateTime? FechaDeNacimiento { get; set; }
        public int? Edad { get; set; }
    }

    public class CrearRolDto
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Descripcion { get; set; } = string.Empty;
    }

    public class ActualizarRolDto
    {
        [StringLength(50)]
        public string? Nombre { get; set; }
        
        [StringLength(200)]
        public string? Descripcion { get; set; }
        
        public bool? EstaActivo { get; set; }
    }

    public class RolDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime CreadoEn { get; set; }
        public bool EstaActivo { get; set; }
        public int CantidadUsuarios { get; set; }
    }
}
