using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPG.Models
{
    public enum TipoDeEventoDeUso
    {
        ConfirmacionDeUso,      // Confirmación de uso por parte del voluntario
        ReporteDeIncidente,     // Reporte de incidente
        ObservacionDeEstado,    // Observación del estado del material
        FinalizacionDeUso,      // Finalización del uso
        ReporteDeDanio,         // Reporte de daño
        VerificacionDeCalidad,  // Verificación de calidad
        ProcesoDeDevolucion     // Proceso de devolución
    }

    public enum CondicionDelRecurso
    {
        Excelente,      // Excelente
        Bueno,          // Bueno
        Regular,        // Regular
        Malo,           // Malo
        Daniado,        // Dañado
        Inservible      // Inservible
    }

    public class RegistroDeUsoDeRecurso
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdAsignacionDeRecurso { get; set; }

        [Required]
        public TipoDeEventoDeUso TipoDeEvento { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty; // Título del evento/incidente

        [Required]
        [StringLength(2000)]
        public string Descripcion { get; set; } = string.Empty; // Descripción detallada

        public CondicionDelRecurso? CondicionAntes { get; set; } // Estado antes del uso

        public CondicionDelRecurso? CondicionDespues { get; set; } // Estado después del uso

        public int? CantidadAfectada { get; set; } // Cantidad afectada en caso de incidente

        [StringLength(1000)]
        public string? AccionesTomadas { get; set; } // Acciones tomadas para resolver el incidente

        [StringLength(1000)]
        public string? Recomendaciones { get; set; } // Recomendaciones para el futuro

        public bool RequiereSeguimiento { get; set; } = false; // Si requiere seguimiento

        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        public int ReportadoPorIdUsuario { get; set; } // Usuario que reporta el evento

        [StringLength(500)]
        public string? UrlsDeFotos { get; set; } // URLs de fotos del incidente (separadas por comas)

        public bool EstaResuelto { get; set; } = false; // Si el incidente está resuelto

        public DateTime? ResueltoEn { get; set; }

        public int? ResueltoPorIdUsuario { get; set; }

        [StringLength(1000)]
        public string? NotasDeResolucion { get; set; }

        // Navigation properties
        [ForeignKey("IdAsignacionDeRecurso")]
        public AsignacionDeRecurso AsignacionDeRecurso { get; set; } = null!;

        [ForeignKey("ReportadoPorIdUsuario")]
        public Usuario ReportadoPor { get; set; } = null!;

        [ForeignKey("ResueltoPorIdUsuario")]
        public Usuario? ResueltoPor { get; set; }
    }
}