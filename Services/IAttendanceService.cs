using ApiPG.DTOs;

namespace ApiPG.Services
{
    public interface IAttendanceService
    {
        Task<AttendanceReportDto?> AddAsync(CreateAttendanceDto dto);
        Task<IEnumerable<AttendanceReportDto>> GetByLevelAndDateAsync(int levelId, DateTime date);
        Task<IEnumerable<AttendanceReportDto>> GetByUserAsync(int userId, DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<AttendanceReportDto>> GetReportAsync(DateTime? from = null, DateTime? to = null, int? levelId = null);
    }
}
