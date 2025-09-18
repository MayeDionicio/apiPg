using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPG.Models
{
    public class Resource
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty; // Ej: "Materiales", "Temperas", "Herramientas", etc.

        [Required]
        public int Quantity { get; set; } // Cantidad total disponible

        [Required]
        public int AvailableQuantity { get; set; } // Cantidad disponible actualmente

        [StringLength(50)]
        public string? Unit { get; set; } // Unidad de medida: "piezas", "litros", "metros", etc.

        [StringLength(200)]
        public string? Location { get; set; } // Ubicación del recurso

        [Column(TypeName = "decimal(10,2)")]
        public decimal? EstimatedValue { get; set; } // Valor estimado del recurso

        [StringLength(1000)]
        public string? Notes { get; set; } // Notas adicionales o instrucciones de uso

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public bool IsActive { get; set; } = true; // No se pueden borrar, solo desactivar

        // Navigation properties
        public ICollection<ResourceAssignment> ResourceAssignments { get; set; } = new List<ResourceAssignment>();
    }
}