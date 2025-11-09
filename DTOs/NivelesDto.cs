using System.ComponentModel.DataAnnotations;

namespace ApiPG.DTOs
{
    public class CrearNivelDto
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public decimal EdadMinima { get; set; }

        [Required]
        public decimal EdadMaxima { get; set; }

        // TutorId opcional al crear
        public int? VoluntarioId { get; set; }
        
        [StringLength(500)]
        public string? Descripcion { get; set; }
    }

    public class ActualizarNivelDto
    {
        [StringLength(100)]
        public string? Nombre { get; set; }

        public decimal? EdadMinima { get; set; }
        
        public decimal? EdadMaxima { get; set; }

        // Cambiar tutor
        public int? VoluntarioId { get; set; }

        public bool? EstaActivo { get; set; }
        
        [StringLength(500)]
        public string? Descripcion { get; set; }
    }

    public class NivelDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal EdadMinima { get; set; }
        public decimal EdadMaxima { get; set; }
        public int? VoluntarioId { get; set; }
        public string? NombreVoluntario { get; set; }
        public DateTime CreadoEn { get; set; }
        public bool EstaActivo { get; set; }
        public int CantidadParticipantes { get; set; }
    }
}
