using System.ComponentModel.DataAnnotations;

namespace ApiPG.Models
{
    /// <summary>
    /// Define un logro que puede obtener un facilitador
    /// Creado por coordinadores
    /// </summary>
    public class LogroFacilitador
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        public string Descripcion { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Categoria { get; set; } // Ej: "Excelencia", "Innovación", "Compromiso"
        
        [StringLength(50)]
        public string? Icono { get; set; } // URL del icono o emoji
        
        [Required]
        public int PuntosValor { get; set; } // Puntos que vale el logro
        
        [StringLength(500)]
        public string? CriteriosObtencion { get; set; } // Cómo se obtiene este logro
        
        // Tipo de logro
        [StringLength(50)]
        public string TipoLogro { get; set; } = "Manual"; // Manual, Automatico
        
        // Si es automático, condiciones
        public int? CantidadActividadesRequeridas { get; set; }
        public int? CantidadEstudiantesRequeridos { get; set; }
        public int? DiasConsecutivosRequeridos { get; set; }
        
        // Metadata
        [Required]
        public int CreadoPorIdUsuario { get; set; } // Coordinador que lo creó
        public Usuario? CreadoPor { get; set; }
        
        public DateTime CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public bool EstaActivo { get; set; }
        
        // Relación con logros obtenidos
        public ICollection<LogroObtenidoFacilitador> LogrosObtenidos { get; set; } = new List<LogroObtenidoFacilitador>();
    }

    /// <summary>
    /// Representa un logro obtenido por un facilitador
    /// </summary>
    public class LogroObtenidoFacilitador
    {
        public int Id { get; set; }
        
        [Required]
        public int IdLogro { get; set; }
        public LogroFacilitador Logro { get; set; } = null!;
        
        [Required]
        public int IdFacilitador { get; set; }
        public Usuario Facilitador { get; set; } = null!;
        
        public DateTime FechaObtencion { get; set; }
        
        // Quién otorgó el logro (coordinador)
        [Required]
        public int OtorgadoPorIdUsuario { get; set; }
        public Usuario? OtorgadoPor { get; set; }
        
        // Contexto de obtención
        public int? IdActividadRelacionada { get; set; }
        public Actividad? ActividadRelacionada { get; set; }
        
        public int? IdActividadMontessoriRelacionada { get; set; }
        public ActividadMontessori? ActividadMontessoriRelacionada { get; set; }
        
        public int? IdActividadPersonalizadaRelacionada { get; set; }
        public ActividadMontessoriPersonalizada? ActividadPersonalizadaRelacionada { get; set; }
        
        [StringLength(1000)]
        public string? Justificacion { get; set; } // Por qué se otorgó
        
        [StringLength(500)]
        public string? ComentarioCoordinador { get; set; }
        
        public bool EsVisible { get; set; } = true; // El facilitador puede verlo
        
        public DateTime CreadoEn { get; set; }
    }
}
