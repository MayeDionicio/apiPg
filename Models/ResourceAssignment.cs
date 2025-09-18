using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPG.Models
{
    public enum AssignmentStatus
    {
        Pending,        // Asignado pero no confirmado por el voluntario
        Confirmed,      // Confirmado por el voluntario
        InUse,          // En uso actualmente
        Returned,       // Devuelto
        Cancelled       // Cancelado
    }

    public class ResourceAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ResourceId { get; set; }

        [Required]
        public int VolunteerId { get; set; } // UserId del voluntario

        [Required]
        public int QuantityAssigned { get; set; }

        [Required]
        public AssignmentStatus Status { get; set; } = AssignmentStatus.Pending;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ConfirmedAt { get; set; } // Cuando el voluntario confirma

        public DateTime? StartedAt { get; set; } // Cuando inicia el uso

        public DateTime? ReturnedAt { get; set; } // Cuando se devuelve

        public DateTime? ExpectedReturnDate { get; set; } // Fecha esperada de devolución

        [StringLength(1000)]
        public string? InitialNotes { get; set; } // Notas al momento de la asignación

        [StringLength(1000)]
        public string? VolunteerNotes { get; set; } // Notas del voluntario al confirmar

        public int? AssignedByUserId { get; set; } // Usuario que asignó el recurso

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("ResourceId")]
        public Resource Resource { get; set; } = null!;

        [ForeignKey("VolunteerId")]
        public User Volunteer { get; set; } = null!;

        [ForeignKey("AssignedByUserId")]
        public User? AssignedByUser { get; set; }

        public ICollection<ResourceUsageLog> UsageLogs { get; set; } = new List<ResourceUsageLog>();
    }
}