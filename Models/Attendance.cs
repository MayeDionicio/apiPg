using System.ComponentModel.DataAnnotations;

namespace ApiPG.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        public int LevelId { get; set; }
        public Level Level { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Date for which attendance is recorded (use UTC date component)
        public DateTime Date { get; set; }

        // true = present, false = absent
        public bool Present { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // We keep records immutable for deletion purposes; soft-delete flag kept but we'll not expose delete endpoint
        public bool IsDeleted { get; set; } = false;
    }
}
