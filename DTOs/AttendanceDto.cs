namespace ApiPG.DTOs
{
    public class CreateAttendanceDto
    {
        public int LevelId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public bool Present { get; set; }
        public string? Remarks { get; set; }
    }

    public class AttendanceReportDto
    {
        public int Id { get; set; }
        public int LevelId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime Date { get; set; }
        public bool Present { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
