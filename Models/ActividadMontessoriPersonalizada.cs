using System.ComponentModel.DataAnnotations;

namespace ApiPG.Models
{
    /// <summary>
    /// Actividad Montessori personalizada para un estudiante específico
    /// </summary>
    public class ActividadMontessoriPersonalizada
    {
        public int Id { get; set; }
        
        // Relación con la actividad base (opcional, puede ser una actividad nueva)
        public int? IdActividadMontessoriBase { get; set; }
        public ActividadMontessori? ActividadBase { get; set; }
        
        // Estudiante al que se asigna la actividad
        [Required]
        public int IdEstudiante { get; set; }
        public Usuario Estudiante { get; set; } = null!;
        
        // Nivel del estudiante
        public int? IdNivel { get; set; }
        public Nivel? Nivel { get; set; }
        
        // Información personalizada de la actividad
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string AreaPedagogica { get; set; } = string.Empty;
        
        [Required]
        public DateTime FechaAsignacion { get; set; }
        
        public DateTime? FechaInicio { get; set; }
        
        public DateTime? FechaCompletada { get; set; }
        
        public TimeSpan? HoraActividad { get; set; }
        
        [Required]
        public int DuracionMinutosEstimada { get; set; }
        
        public int? DuracionMinutosReal { get; set; }
        
        [Required]
        public string ObjetivoPersonalizado { get; set; } = string.Empty;
        
        // Materiales y presentación personalizados
        public string MaterialesNecesarios { get; set; } = string.Empty;
        
        public string? PresentacionPersonalizada { get; set; }
        
        public string? AdaptacionesEspeciales { get; set; }
        
        // Estado y progreso
        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, EnProgreso, Completada, Cancelada
        
        public int? ProgresosPorcentaje { get; set; } // 0-100
        
        // Evaluación personalizada
        public string? ObservacionesProfesor { get; set; }
        
        public string? LogrosAlcanzados { get; set; } // JSON array
        
        public string? DificultadesEncontradas { get; set; }
        
        public string? SugerenciasParaSeguimiento { get; set; }
        
        [StringLength(50)]
        public string? NivelDesempeno { get; set; } // Excelente, Bueno, Regular, NecesitaApoyo
        
        public bool RequiereRefuerzo { get; set; }
        
        // Evidencias
        public string? EvidenciaUrlFoto { get; set; }
        
        public string? EvidenciaUrlVideo { get; set; }
        
        public string? NotasAdicionales { get; set; }
        
        // Recordatorios y notificaciones
        public bool EnviarRecordatorio { get; set; }
        
        public DateTime? FechaRecordatorio { get; set; }
        
        public bool RecordatorioEnviado { get; set; }
        
        // Prioridad
        [StringLength(20)]
        public string Prioridad { get; set; } = "Normal"; // Alta, Normal, Baja
        
        // Metadata
        [Required]
        public int AsignadoPorIdUsuario { get; set; }
        public Usuario? AsignadoPor { get; set; }
        
        public DateTime CreadoEn { get; set; }
        
        public DateTime? ActualizadoEn { get; set; }
        
        public bool EstaActivo { get; set; }
        
        // Repetición (para actividades recurrentes)
        public bool EsRecurrente { get; set; }
        
        [StringLength(50)]
        public string? FrecuenciaRecurrencia { get; set; } // Diaria, Semanal, Mensual
        
        public DateTime? FechaFinRecurrencia { get; set; }
    }
}
