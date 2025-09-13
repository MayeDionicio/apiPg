using System.ComponentModel.DataAnnotations;

namespace ApiPG.DTOs
{
    public class CreateLevelDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // TutorId opcional al crear
        public int? TutorId { get; set; }
        
    [StringLength(500)]
    public string? Description { get; set; }
    }

    public class UpdateLevelDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        // Cambiar tutor
        public int? TutorId { get; set; }

        public bool? IsActive { get; set; }
        
    [StringLength(500)]
    public string? Description { get; set; }
    }

    public class LevelDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
        public int? TutorId { get; set; }
        public string? TutorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int ParticipantsCount { get; set; }
    }
}
