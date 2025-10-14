using ApiPG.DTOs;

namespace ApiPG.Services
{
    public interface IAttendanceService
    {
        Task<ReporteAsistenciaDto?> AddAsync(CrearAsistenciaDto dto);
        Task<IEnumerable<ReporteAsistenciaDto>> GetByLevelAndDateAsync(int levelId, DateTime date);
        Task<IEnumerable<ReporteAsistenciaDto>> GetByUserAsync(int userId, DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<ReporteAsistenciaDto>> GetReportAsync(DateTime? from = null, DateTime? to = null, int? levelId = null);
    }
}
