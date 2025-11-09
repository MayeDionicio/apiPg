using System.ComponentModel.DataAnnotations;

namespace ApiPG.DTOs
{
    // DTO para crear una actividad personalizada
    public class CrearActividadPersonalizadaDto
    {
        public int? IdActividadMontessoriBase { get; set; }
        
        [Required(ErrorMessage = "El ID del estudiante es requerido")]
        public int IdEstudiante { get; set; }
        
        public int? IdNivel { get; set; }
        
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El área pedagógica es requerida")]
        [StringLength(100)]
        public string AreaPedagogica { get; set; } = string.Empty;
        
        public DateTime? FechaInicio { get; set; }
        
        public TimeSpan? HoraActividad { get; set; }
        
        [Required(ErrorMessage = "La duración estimada es requerida")]
        [Range(1, 480, ErrorMessage = "La duración debe estar entre 1 y 480 minutos")]
        public int DuracionMinutosEstimada { get; set; }
        
        [Required(ErrorMessage = "El objetivo personalizado es requerido")]
        public string ObjetivoPersonalizado { get; set; } = string.Empty;
        
        public string? MaterialesNecesarios { get; set; }
        
        public string? PresentacionPersonalizada { get; set; }
        
        public string? AdaptacionesEspeciales { get; set; }
        
        public string Prioridad { get; set; } = "Normal"; // Alta, Normal, Baja
        
        public bool EnviarRecordatorio { get; set; }
        
        public DateTime? FechaRecordatorio { get; set; }
        
        public bool EsRecurrente { get; set; }
        
        public string? FrecuenciaRecurrencia { get; set; }
        
        public DateTime? FechaFinRecurrencia { get; set; }
        
        public string? NotasAdicionales { get; set; }
    }

    // DTO para actualizar una actividad personalizada
    public class ActualizarActividadPersonalizadaDto
    {
        [StringLength(200)]
        public string? Nombre { get; set; }
        
        public DateTime? FechaInicio { get; set; }
        
        public TimeSpan? HoraActividad { get; set; }
        
        [Range(1, 480)]
        public int? DuracionMinutosEstimada { get; set; }
        
        public string? ObjetivoPersonalizado { get; set; }
        
        public string? MaterialesNecesarios { get; set; }
        
        public string? PresentacionPersonalizada { get; set; }
        
        public string? AdaptacionesEspeciales { get; set; }
        
        public string? Prioridad { get; set; }
        
        public bool? EnviarRecordatorio { get; set; }
        
        public DateTime? FechaRecordatorio { get; set; }
        
        public string? NotasAdicionales { get; set; }
    }

    // DTO para actualizar el estado y progreso
    public class ActualizarProgresoActividadDto
    {
        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(50)]
        public string Estado { get; set; } = string.Empty; // Pendiente, EnProgreso, Completada, Cancelada
        
        [Range(0, 100, ErrorMessage = "El progreso debe estar entre 0 y 100")]
        public int? ProgresosPorcentaje { get; set; }
        
        public DateTime? FechaCompletada { get; set; }
        
        public int? DuracionMinutosReal { get; set; }
        
        public string? ObservacionesProfesor { get; set; }
        
        public List<string>? LogrosAlcanzados { get; set; }
        
        public string? DificultadesEncontradas { get; set; }
        
        public string? SugerenciasParaSeguimiento { get; set; }
        
        public string? NivelDesempeno { get; set; } // Excelente, Bueno, Regular, NecesitaApoyo
        
        public bool? RequiereRefuerzo { get; set; }
        
        public string? EvidenciaUrlFoto { get; set; }
        
        public string? EvidenciaUrlVideo { get; set; }
    }

    // DTO para respuesta de actividad personalizada
    public class ActividadPersonalizadaDto
    {
        public int Id { get; set; }
        public int? IdActividadMontessoriBase { get; set; }
        public string? NombreActividadBase { get; set; }
        public int IdEstudiante { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string EmailEstudiante { get; set; } = string.Empty;
        public int? IdNivel { get; set; }
        public string? NombreNivel { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string AreaPedagogica { get; set; } = string.Empty;
        public DateTime FechaAsignacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaCompletada { get; set; }
        public TimeSpan? HoraActividad { get; set; }
        public int DuracionMinutosEstimada { get; set; }
        public int? DuracionMinutosReal { get; set; }
        public string ObjetivoPersonalizado { get; set; } = string.Empty;
        public string MaterialesNecesarios { get; set; } = string.Empty;
        public string? PresentacionPersonalizada { get; set; }
        public string? AdaptacionesEspeciales { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int? ProgresosPorcentaje { get; set; }
        public string? ObservacionesProfesor { get; set; }
        public List<string>? LogrosAlcanzados { get; set; }
        public string? DificultadesEncontradas { get; set; }
        public string? SugerenciasParaSeguimiento { get; set; }
        public string? NivelDesempeno { get; set; }
        public bool RequiereRefuerzo { get; set; }
        public string? EvidenciaUrlFoto { get; set; }
        public string? EvidenciaUrlVideo { get; set; }
        public string? NotasAdicionales { get; set; }
        public bool EnviarRecordatorio { get; set; }
        public DateTime? FechaRecordatorio { get; set; }
        public bool RecordatorioEnviado { get; set; }
        public string Prioridad { get; set; } = string.Empty;
        public int AsignadoPorIdUsuario { get; set; }
        public string NombreAsignadoPor { get; set; } = string.Empty;
        public DateTime CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public bool EstaActivo { get; set; }
        public bool EsRecurrente { get; set; }
        public string? FrecuenciaRecurrencia { get; set; }
        public DateTime? FechaFinRecurrencia { get; set; }
    }

    // DTO para filtros de búsqueda
    public class FiltrosActividadPersonalizadaDto
    {
        public int? IdEstudiante { get; set; }
        public int? IdNivel { get; set; }
        public string? Estado { get; set; }
        public string? AreaPedagogica { get; set; }
        public string? Prioridad { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public bool? RequiereRefuerzo { get; set; }
        public bool? SoloActivas { get; set; } = true;
        public int Pagina { get; set; } = 1;
        public int RegistrosPorPagina { get; set; } = 20;
    }

    // DTO para estadísticas del estudiante
    public class EstadisticasActividadesEstudianteDto
    {
        public int IdEstudiante { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public int TotalActividades { get; set; }
        public int ActividadesPendientes { get; set; }
        public int ActividadesEnProgreso { get; set; }
        public int ActividadesCompletadas { get; set; }
        public int ActividadesCanceladas { get; set; }
        public double PromedioProgreso { get; set; }
        public int ActividadesConRefuerzo { get; set; }
        public Dictionary<string, int>? ActividadesPorArea { get; set; }
        public Dictionary<string, int>? ActividadesPorNivelDesempeno { get; set; }
        public DateTime? UltimaActividadCompletada { get; set; }
        public int TotalMinutosTrabajados { get; set; }
    }
}
