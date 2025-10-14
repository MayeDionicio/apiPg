using System.ComponentModel.DataAnnotations;

namespace ApiPG.Models
{
    public class Rol
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Descripcion { get; set; } = string.Empty;
        
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        
        public bool EstaActivo { get; set; } = true;
        
        // Navegación
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
