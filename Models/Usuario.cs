using System.ComponentModel.DataAnnotations;

namespace ApiPG.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string PrimerNombre { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string CorreoElectronico { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string NombreDeUsuario { get; set; } = string.Empty;
        
        [Required]
        public string HashDeContrasena { get; set; } = string.Empty;
        
        public int IdRol { get; set; }
        public Rol Rol { get; set; } = null!;
        
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime? ActualizadoEn { get; set; }
        
        public bool EstaActivo { get; set; } = true;
        
        public string NombreCompleto => $"{PrimerNombre} {Apellido}";
    }
}
