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

        public async Task<ReporteAsistenciaDto?> AddAsync(CrearAsistenciaDto dto)
        {
            // Normalize date to date component (UTC)
            var dateOnly = dto.Fecha.Date;

            var attendance = new Asistencia
            {
                IdNivel = dto.NivelId,
                IdUsuario = dto.UsuarioId,
                Fecha = dateOnly,
                Presente = dto.Presente,
                Observaciones = dto.Observaciones,
                CreadoEn = DateTime.UtcNow,
                EstaEliminado = false
            };

            _db.Attendances.Add(attendance);
            await _db.SaveChangesAsync();

            // load user
            await _db.Entry(attendance).Reference(a => a.Usuario).LoadAsync();

            return new ReporteAsistenciaDto
            {
                Id = attendance.Id,
                NivelId = attendance.IdNivel,
                UsuarioId = attendance.IdUsuario,
                NombreUsuario = attendance.Usuario.NombreCompleto,
                Fecha = attendance.Fecha,
                Presente = attendance.Presente,
                Observaciones = attendance.Observaciones,
                CreadoEn = attendance.CreadoEn
            };
        }

        public async Task<IEnumerable<ReporteAsistenciaDto>> GetByLevelAndDateAsync(int levelId, DateTime date)
        {
            var d = date.Date;
            return await _db.Attendances
                .Where(a => !a.EstaEliminado && a.IdNivel == levelId && a.Fecha == d)
                .Include(a => a.Usuario)
                .Select(a => new ReporteAsistenciaDto
                {
                    Id = a.Id,
                    NivelId = a.IdNivel,
                    UsuarioId = a.IdUsuario,
                    NombreUsuario = a.Usuario.NombreCompleto,
                    Fecha = a.Fecha,
                    Presente = a.Presente,
                    Observaciones = a.Observaciones,
                    CreadoEn = a.CreadoEn
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ReporteAsistenciaDto>> GetByUserAsync(int userId, DateTime? from = null, DateTime? to = null)
        {
            var q = _db.Attendances
                .Where(a => !a.EstaEliminado && a.IdUsuario == userId);

            if (from.HasValue) q = q.Where(a => a.Fecha >= from.Value.Date);
            if (to.HasValue) q = q.Where(a => a.Fecha <= to.Value.Date);

            q = q.Include(a => a.Usuario);

            return await q.Select(a => new ReporteAsistenciaDto
            {
                Id = a.Id,
                NivelId = a.IdNivel,
                UsuarioId = a.IdUsuario,
                NombreUsuario = a.Usuario.NombreCompleto,
                Fecha = a.Fecha,
                Presente = a.Presente,
                Observaciones = a.Observaciones,
                CreadoEn = a.CreadoEn
            }).ToListAsync();
        }

        public async Task<IEnumerable<ReporteAsistenciaDto>> GetReportAsync(DateTime? from = null, DateTime? to = null, int? levelId = null)
        {
            var q = _db.Attendances.Where(a => !a.EstaEliminado);
            if (levelId.HasValue) q = q.Where(a => a.IdNivel == levelId.Value);
            if (from.HasValue) q = q.Where(a => a.Fecha >= from.Value.Date);
            if (to.HasValue) q = q.Where(a => a.Fecha <= to.Value.Date);

            q = q.Include(a => a.Usuario);

            return await q.Select(a => new ReporteAsistenciaDto
            {
                Id = a.Id,
                NivelId = a.IdNivel,
                UsuarioId = a.IdUsuario,
                NombreUsuario = a.Usuario.NombreCompleto,
                Fecha = a.Fecha,
                Presente = a.Presente,
                Observaciones = a.Observaciones,
                CreadoEn = a.CreadoEn
            }).ToListAsync();
        }
    }
}
