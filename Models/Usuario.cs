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
        
        // Código único del participante (opcional, solo para participantes)
        [StringLength(50)]
        public string? CodigoParticipante { get; set; }
        
        // Fecha de nacimiento (solo para participantes)
        public DateTime? FechaDeNacimiento { get; set; }
        
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime? ActualizadoEn { get; set; }
        
        public bool EstaActivo { get; set; } = true;
        
        public string NombreCompleto => $"{PrimerNombre} {Apellido}";
        
        // Propiedad calculada para obtener la edad
        public int? Edad 
        { 
            get 
            {
                if (!FechaDeNacimiento.HasValue)
                    return null;
                    
                var hoy = DateTime.Today;
                var edad = hoy.Year - FechaDeNacimiento.Value.Year;
                
                // Restar un año si aún no ha cumplido años este año
                if (FechaDeNacimiento.Value.Date > hoy.AddYears(-edad))
                    edad--;
                    
                return edad;
            }
        }
    }
}
