using System.ComponentModel.DataAnnotations;

namespace ApiPG.Models
{
    public class PerfilUsuario
    {
        public int Id { get; set; }
        
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;
        
        // Foto de perfil (URL o ruta)
        [StringLength(500)]
        public string? FotoPerfil { get; set; }
        
        // Información adicional
        [StringLength(20)]
        public string? Telefono { get; set; }
        
        [StringLength(200)]
        public string? Direccion { get; set; }
        
        [StringLength(100)]
        public string? Ciudad { get; set; }
        
        [StringLength(50)]
        public string? Pais { get; set; }
        
        // Información de contacto de emergencia
        [StringLength(100)]
        public string? ContactoEmergenciaNombre { get; set; }
        
        [StringLength(20)]
        public string? ContactoEmergenciaTelefono { get; set; }
        
        [StringLength(50)]
        public string? ContactoEmergenciaRelacion { get; set; }
        
        // Información adicional para participantes
        [StringLength(100)]
        public string? NombrePadre { get; set; }
        
        [StringLength(100)]
        public string? NombreMadre { get; set; }
        
        [StringLength(100)]
        public string? NombreTutor { get; set; }
        
        // Información médica básica
        [StringLength(500)]
        public string? Alergias { get; set; }
        
        [StringLength(500)]
        public string? CondicionesMedicas { get; set; }
        
        [StringLength(500)]
        public string? Medicamentos { get; set; }
        
        // Biografía o descripción
        [StringLength(1000)]
        public string? Biografia { get; set; }
        
        // Intereses y hobbies
        [StringLength(500)]
        public string? Intereses { get; set; }
        
        // Redes sociales (opcional)
        [StringLength(100)]
        public string? Facebook { get; set; }
        
        [StringLength(100)]
        public string? Instagram { get; set; }
        
        [StringLength(100)]
        public string? Twitter { get; set; }
        
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime? ActualizadoEn { get; set; }
        
        public bool EstaActivo { get; set; } = true;
    }
}
