using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPG.Models
{
    public class Recurso
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        [StringLength(50)]
        public string Categoria { get; set; } = string.Empty; // Ej: "Materiales", "Temperas", "Herramientas", etc.

        [Required]
        public int Cantidad { get; set; } // Cantidad total disponible

        [Required]
        public int CantidadDisponible { get; set; } // Cantidad disponible actualmente

        [StringLength(50)]
        public string? Unidad { get; set; } // Unidad de medida: "piezas", "litros", "metros", etc.

        [StringLength(200)]
        public string? Ubicacion { get; set; } // Ubicación del recurso

        [Column(TypeName = "decimal(10,2)")]
        public decimal? ValorEstimado { get; set; } // Valor estimado del recurso

        [StringLength(1000)]
        public string? Notas { get; set; } // Notas adicionales o instrucciones de uso

        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        public DateTime? ActualizadoEn { get; set; }

        [Required]
        public bool EstaActivo { get; set; } = true; // No se pueden borrar, solo desactivar

        // Navigation properties
        public ICollection<AsignacionDeRecurso> AsignacionesDeRecurso { get; set; } = new List<AsignacionDeRecurso>();
    }
}