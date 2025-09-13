namespace ApiPG.DTOs
{
    public class LevelParticipantDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
