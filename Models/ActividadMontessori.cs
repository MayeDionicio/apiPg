namespace ApiPG.Models
{
    public class ActividadMontessori
    {
        public int Id { get; set; }
        
        // Sección 1: Identificación
        public string Nombre { get; set; } = string.Empty;
        public string AreaPedagogica { get; set; } = string.Empty;
        public DateTime FechaActividad { get; set; }
        public TimeSpan? HoraActividad { get; set; }
        public decimal EdadMinima { get; set; }
        public decimal EdadMaxima { get; set; }
        public int DuracionMinutos { get; set; }
        public string ObjetivoEspecifico { get; set; } = string.Empty;
        
        // Sección 2: Materiales y Ambiente
        public string MaterialesNecesarios { get; set; } = string.Empty;
        public string MontajeAmbiente { get; set; } = string.Empty;
        public string? Prerequisitos { get; set; }
        
        // Sección 3: Presentación y Control
        public string PresentacionPasoAPaso { get; set; } = string.Empty;
        public string ControlDelError { get; set; } = string.Empty;
        
        // Sección 4: Autonomía y Normas
        public string? AspectosAutonomia { get; set; } // JSON array de strings
        public string? LimitesYNormas { get; set; }
        
        // Sección 5: Evaluación
        public string IndicadoresDeLogro { get; set; } = string.Empty;
        public string? AdaptacionesVariaciones { get; set; }
        public string? NivelDificultad { get; set; }
        public string? EvidenciaUrl { get; set; }
        public string? ObservacionesAdicionales { get; set; }
        public string? ChecklistMontessori { get; set; } // JSON array de strings
        
        // Metadata
        public int CreadoPorIdUsuario { get; set; }
        public Usuario? CreadoPor { get; set; }
        public DateTime CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public bool EstaActivo { get; set; }
        
        // Relación con logros
        public ICollection<LogroMontessori> Logros { get; set; } = new List<LogroMontessori>();
    }

    public class LogroMontessori
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty; // Emoji o clase de icono
        public bool EsObtenido { get; set; }
        public DateTime? FechaObtencion { get; set; }
        
        // Relación con actividad y usuario
        public int ActividadMontessoriId { get; set; }
        public ActividadMontessori? ActividadMontessori { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        
        // Metadata
        public DateTime CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public bool EstaActivo { get; set; }
    }
}
