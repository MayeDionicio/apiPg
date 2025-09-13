using ApiPG.Data;
using ApiPG.DTOs;
using ApiPG.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPG.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ApiPGContext _db;

        public AttendanceService(ApiPGContext db)
        {
            _db = db;
        }

        public async Task<AttendanceReportDto?> AddAsync(CreateAttendanceDto dto)
        {
            // Normalize date to date component (UTC)
            var dateOnly = dto.Date.Date;

            var attendance = new Attendance
            {
                LevelId = dto.LevelId,
                UserId = dto.UserId,
                Date = dateOnly,
                Present = dto.Present,
                Remarks = dto.Remarks,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _db.Attendances.Add(attendance);
            await _db.SaveChangesAsync();

            // load user
            await _db.Entry(attendance).Reference(a => a.User).LoadAsync();

            return new AttendanceReportDto
            {
                Id = attendance.Id,
                LevelId = attendance.LevelId,
                UserId = attendance.UserId,
                UserName = attendance.User.FullName,
                Date = attendance.Date,
                Present = attendance.Present,
                Remarks = attendance.Remarks,
                CreatedAt = attendance.CreatedAt
            };
        }

        public async Task<IEnumerable<AttendanceReportDto>> GetByLevelAndDateAsync(int levelId, DateTime date)
        {
            var d = date.Date;
            return await _db.Attendances
                .Where(a => !a.IsDeleted && a.LevelId == levelId && a.Date == d)
                .Include(a => a.User)
                .Select(a => new AttendanceReportDto
                {
                    Id = a.Id,
                    LevelId = a.LevelId,
                    UserId = a.UserId,
                    UserName = a.User.FullName,
                    Date = a.Date,
                    Present = a.Present,
                    Remarks = a.Remarks,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AttendanceReportDto>> GetByUserAsync(int userId, DateTime? from = null, DateTime? to = null)
        {
            var q = _db.Attendances
                .Where(a => !a.IsDeleted && a.UserId == userId);

            if (from.HasValue) q = q.Where(a => a.Date >= from.Value.Date);
            if (to.HasValue) q = q.Where(a => a.Date <= to.Value.Date);

            q = q.Include(a => a.User);

            return await q.Select(a => new AttendanceReportDto
            {
                Id = a.Id,
                LevelId = a.LevelId,
                UserId = a.UserId,
                UserName = a.User.FullName,
                Date = a.Date,
                Present = a.Present,
                Remarks = a.Remarks,
                CreatedAt = a.CreatedAt
            }).ToListAsync();
        }

        public async Task<IEnumerable<AttendanceReportDto>> GetReportAsync(DateTime? from = null, DateTime? to = null, int? levelId = null)
        {
            var q = _db.Attendances.Where(a => !a.IsDeleted);
            if (levelId.HasValue) q = q.Where(a => a.LevelId == levelId.Value);
            if (from.HasValue) q = q.Where(a => a.Date >= from.Value.Date);
            if (to.HasValue) q = q.Where(a => a.Date <= to.Value.Date);

            q = q.Include(a => a.User);

            return await q.Select(a => new AttendanceReportDto
            {
                Id = a.Id,
                LevelId = a.LevelId,
                UserId = a.UserId,
                UserName = a.User.FullName,
                Date = a.Date,
                Present = a.Present,
                Remarks = a.Remarks,
                CreatedAt = a.CreatedAt
            }).ToListAsync();
        }
    }
}
