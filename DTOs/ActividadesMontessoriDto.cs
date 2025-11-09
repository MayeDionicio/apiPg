namespace ApiPG.DTOs
{
    // ============ DTOs para Actividad Montessori ============
    
    public class CrearActividadMontessoriDto
    {
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
        public List<string>? AspectosAutonomia { get; set; }
        public string? LimitesYNormas { get; set; }
        
        // Sección 5: Evaluación
        public string IndicadoresDeLogro { get; set; } = string.Empty;
        public string? AdaptacionesVariaciones { get; set; }
        public string? NivelDificultad { get; set; }
        public string? EvidenciaUrl { get; set; }
        public string? ObservacionesAdicionales { get; set; }
        public List<string>? ChecklistMontessori { get; set; }
    }

    public class ActualizarActividadMontessoriDto
    {
        public string? Nombre { get; set; }
        public string? AreaPedagogica { get; set; }
        public DateTime? FechaActividad { get; set; }
        public TimeSpan? HoraActividad { get; set; }
        public decimal? EdadMinima { get; set; }
        public decimal? EdadMaxima { get; set; }
        public int? DuracionMinutos { get; set; }
        public string? ObjetivoEspecifico { get; set; }
        public string? MaterialesNecesarios { get; set; }
        public string? MontajeAmbiente { get; set; }
        public string? Prerequisitos { get; set; }
        public string? PresentacionPasoAPaso { get; set; }
        public string? ControlDelError { get; set; }
        public List<string>? AspectosAutonomia { get; set; }
        public string? LimitesYNormas { get; set; }
        public string? IndicadoresDeLogro { get; set; }
        public string? AdaptacionesVariaciones { get; set; }
        public string? NivelDificultad { get; set; }
        public string? EvidenciaUrl { get; set; }
        public string? ObservacionesAdicionales { get; set; }
        public List<string>? ChecklistMontessori { get; set; }
    }

    public class ActividadMontessoriDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string AreaPedagogica { get; set; } = string.Empty;
        public DateTime FechaActividad { get; set; }
        public TimeSpan? HoraActividad { get; set; }
        public decimal EdadMinima { get; set; }
        public decimal EdadMaxima { get; set; }
        public int DuracionMinutos { get; set; }
        public string ObjetivoEspecifico { get; set; } = string.Empty;
        public string MaterialesNecesarios { get; set; } = string.Empty;
        public string MontajeAmbiente { get; set; } = string.Empty;
        public string? Prerequisitos { get; set; }
        public string PresentacionPasoAPaso { get; set; } = string.Empty;
        public string ControlDelError { get; set; } = string.Empty;
        public List<string>? AspectosAutonomia { get; set; }
        public string? LimitesYNormas { get; set; }
        public string IndicadoresDeLogro { get; set; } = string.Empty;
        public string? AdaptacionesVariaciones { get; set; }
        public string? NivelDificultad { get; set; }
        public string? EvidenciaUrl { get; set; }
        public string? ObservacionesAdicionales { get; set; }
        public List<string>? ChecklistMontessori { get; set; }
        public int CreadoPorIdUsuario { get; set; }
        public string NombreVoluntario { get; set; } = string.Empty;
        public List<LogroMontessoriDto> Logros { get; set; } = new List<LogroMontessoriDto>();
        public DateTime CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public bool EstaActivo { get; set; }
    }

    // ============ DTOs para Logro Montessori ============
    
    public class CrearLogroMontessoriDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Icono { get; set; } = "⭐";
        public int ActividadMontessoriId { get; set; }
        public int UsuarioId { get; set; }
    }

    public class ActualizarLogroMontessoriDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Icono { get; set; }
        public bool? EsObtenido { get; set; }
    }

    public class LogroMontessoriDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public bool EsObtenido { get; set; }
        public DateTime? FechaObtencion { get; set; }
        public int ActividadMontessoriId { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime CreadoEn { get; set; }
        public bool EstaActivo { get; set; }
    }

    public class EstadisticasActividadesMontessoriDto
    {
        public int TotalActividades { get; set; }
        public int ActividadesPorArea { get; set; }
        public int LogrosObtenidos { get; set; }
        public int LogrosEnProgreso { get; set; }
        public double PromedioEdadMinima { get; set; }
        public double PromedioDuracion { get; set; }
        public Dictionary<string, int> ActividadesPorAreaDetalle { get; set; } = new Dictionary<string, int>();
    }
}
