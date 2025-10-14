using System.ComponentModel.DataAnnotations;

namespace ApiPG.DTOs
{
    public class CrearProductoDto
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;
        
        [Range(0, double.MaxValue)]
        public decimal Precio { get; set; }
    }

    public class ActualizarProductoDto
    {
        [StringLength(100)]
        public string? Nombre { get; set; }
        
        [StringLength(500)]
        public string? Descripcion { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal? Precio { get; set; }
        
        public bool? EstaActivo { get; set; }
    }

    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public DateTime CreadoEn { get; set; }
        public bool EstaActivo { get; set; }
    }
}
