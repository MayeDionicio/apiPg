namespace ApiPG.Models
{
    public class LevelParticipant
    {
        public int LevelId { get; set; }
        public Level Level { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true; // Para soft-assignment
    }
}
