using System.ComponentModel.DataAnnotations;

namespace ApiPG.Models
{
    public class Level
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

        // Tutor del nivel: un voluntario (User)
        public int? TutorId { get; set; }
        public User? Tutor { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Participantes (many-to-many a través de LevelParticipant)
        public ICollection<LevelParticipant> LevelParticipants { get; set; } = new List<LevelParticipant>();
    }
}
