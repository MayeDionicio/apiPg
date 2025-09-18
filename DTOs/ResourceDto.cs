using System.ComponentModel.DataAnnotations;
using ApiPG.Models;

namespace ApiPG.DTOs
{
    // DTOs para Resource
    public class CreateResourceDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }

        [StringLength(50)]
        public string? Unit { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El valor estimado debe ser positivo")]
        public decimal? EstimatedValue { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class UpdateResourceDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int? Quantity { get; set; }

        [StringLength(50)]
        public string? Unit { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El valor estimado debe ser positivo")]
        public decimal? EstimatedValue { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public bool? IsActive { get; set; }
    }

    public class ResourceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string? Unit { get; set; }
        public string? Location { get; set; }
        public decimal? EstimatedValue { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public int AssignmentsCount { get; set; } // Número total de asignaciones
        public int ActiveAssignmentsCount { get; set; } // Asignaciones activas
    }

    // DTOs para ResourceAssignment
    public class CreateResourceAssignmentDto
    {
        [Required]
        public int ResourceId { get; set; }

        [Required]
        public int VolunteerId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int QuantityAssigned { get; set; }

        public DateTime? ExpectedReturnDate { get; set; }

        [StringLength(1000)]
        public string? InitialNotes { get; set; }
    }

    public class UpdateResourceAssignmentDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int? QuantityAssigned { get; set; }

        public AssignmentStatus? Status { get; set; }

        public DateTime? ExpectedReturnDate { get; set; }

        [StringLength(1000)]
        public string? InitialNotes { get; set; }

        [StringLength(1000)]
        public string? VolunteerNotes { get; set; }
    }

    public class ConfirmAssignmentDto
    {
        [StringLength(1000)]
        public string? VolunteerNotes { get; set; }
    }

    public class ResourceAssignmentDto
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public string ResourceName { get; set; } = string.Empty;
        public string ResourceCategory { get; set; } = string.Empty;
        public int VolunteerId { get; set; }
        public string VolunteerName { get; set; } = string.Empty;
        public int QuantityAssigned { get; set; }
        public AssignmentStatus Status { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public string? InitialNotes { get; set; }
        public string? VolunteerNotes { get; set; }
        public int? AssignedByUserId { get; set; }
        public string? AssignedByUserName { get; set; }
        public bool IsActive { get; set; }
        public int UsageLogsCount { get; set; }
        public bool HasIncidents { get; set; }
    }

    // DTOs para ResourceUsageLog
    public class CreateResourceUsageLogDto
    {
        [Required]
        public int ResourceAssignmentId { get; set; }

        [Required]
        public UsageEventType EventType { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        public ResourceCondition? ConditionBefore { get; set; }

        public ResourceCondition? ConditionAfter { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad afectada debe ser mayor a 0")]
        public int? QuantityAffected { get; set; }

        [StringLength(1000)]
        public string? ActionsTaken { get; set; }

        [StringLength(1000)]
        public string? Recommendations { get; set; }

        public bool RequiresFollowUp { get; set; } = false;

        [StringLength(500)]
        public string? PhotoUrls { get; set; }
    }

    public class ResourceUsageLogDto
    {
        public int Id { get; set; }
        public int ResourceAssignmentId { get; set; }
        public string ResourceName { get; set; } = string.Empty;
        public string VolunteerName { get; set; } = string.Empty;
        public UsageEventType EventType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ResourceCondition? ConditionBefore { get; set; }
        public ResourceCondition? ConditionAfter { get; set; }
        public int? QuantityAffected { get; set; }
        public string? ActionsTaken { get; set; }
        public string? Recommendations { get; set; }
        public bool RequiresFollowUp { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ReportedByUserId { get; set; }
        public string ReportedByUserName { get; set; } = string.Empty;
        public string? PhotoUrls { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int? ResolvedByUserId { get; set; }
        public string? ResolvedByUserName { get; set; }
        public string? ResolutionNotes { get; set; }
    }

    public class ResolveIncidentDto
    {
        [Required]
        [StringLength(1000)]
        public string ResolutionNotes { get; set; } = string.Empty;
    }
}