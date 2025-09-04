using System.ComponentModel.DataAnnotations;

namespace ApiPG.Models
{
    public class Role
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Navegación
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
