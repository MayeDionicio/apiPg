using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPG.Models
{
    public enum UsageEventType
    {
        ConfirmationOfUse,      // Confirmación de uso por parte del voluntario
        IncidentReport,         // Reporte de incidente
        StateObservation,       // Observación del estado del material
        UsageCompletion,        // Finalización del uso
        DamageReport,           // Reporte de daño
        QualityCheck,           // Verificación de calidad
        ReturnProcess           // Proceso de devolución
    }

    public enum ResourceCondition
    {
        Excellent,      // Excelente
        Good,           // Bueno
        Fair,           // Regular
        Poor,           // Malo
        Damaged,        // Dañado
        Unusable        // Inservible
    }

    public class ResourceUsageLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ResourceAssignmentId { get; set; }

        [Required]
        public UsageEventType EventType { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty; // Título del evento/incidente

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty; // Descripción detallada

        public ResourceCondition? ConditionBefore { get; set; } // Estado antes del uso

        public ResourceCondition? ConditionAfter { get; set; } // Estado después del uso

        public int? QuantityAffected { get; set; } // Cantidad afectada en caso de incidente

        [StringLength(1000)]
        public string? ActionsTaken { get; set; } // Acciones tomadas para resolver el incidente

        [StringLength(1000)]
        public string? Recommendations { get; set; } // Recomendaciones para el futuro

        public bool RequiresFollowUp { get; set; } = false; // Si requiere seguimiento

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ReportedByUserId { get; set; } // Usuario que reporta el evento

        [StringLength(500)]
        public string? PhotoUrls { get; set; } // URLs de fotos del incidente (separadas por comas)

        public bool IsResolved { get; set; } = false; // Si el incidente está resuelto

        public DateTime? ResolvedAt { get; set; }

        public int? ResolvedByUserId { get; set; }

        [StringLength(1000)]
        public string? ResolutionNotes { get; set; }

        // Navigation properties
        [ForeignKey("ResourceAssignmentId")]
        public ResourceAssignment ResourceAssignment { get; set; } = null!;

        [ForeignKey("ReportedByUserId")]
        public User ReportedByUser { get; set; } = null!;

        [ForeignKey("ResolvedByUserId")]
        public User? ResolvedByUser { get; set; }
    }
}