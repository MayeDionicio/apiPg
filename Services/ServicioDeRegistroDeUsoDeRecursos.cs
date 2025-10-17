using ApiPG.DTOs;
using ApiPG.Data;
using ApiPG.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPG.Services
{
    public class ResourceUsageLogService : IResourceUsageLogService
    {
        private readonly ApiPGContext _db;

        public ResourceUsageLogService(ApiPGContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetAllAsync()
        {
            var logs = await _db.RegistrosDeUsoDeRecurso
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Recurso)
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Voluntario)
                .Include(l => l.ReportadoPor)
                .Include(l => l.ResueltoPor)
                .OrderByDescending(l => l.CreadoEn)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        public async Task<RegistroDeUsoDeRecursoDto?> GetByIdAsync(int id)
        {
            var log = await _db.RegistrosDeUsoDeRecurso
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Recurso)
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Voluntario)
                .Include(l => l.ReportadoPor)
                .Include(l => l.ResueltoPor)
                .FirstOrDefaultAsync(l => l.Id == id);

            return log != null ? MapToDto(log) : null;
        }

        public async Task<RegistroDeUsoDeRecursoDto> CreateAsync(CrearRegistroDeUsoDeRecursoDto dto, int reportedByUserId)
        {
            // Verificar que la asignación existe
            var assignment = await _db.AsignacionesDeRecurso.FindAsync(dto.AsignacionDeRecursoId);
            if (assignment == null)
                throw new ArgumentException("La asignación de recurso especificada no existe");

            // Verificar que el usuario existe
            var user = await _db.Usuarios.FindAsync(reportedByUserId);
            if (user == null || !user.EstaActivo)
                throw new ArgumentException("El usuario especificado no existe o no está activo");

            var log = new RegistroDeUsoDeRecurso
            {
                IdAsignacionDeRecurso = dto.AsignacionDeRecursoId,
                TipoDeEvento = dto.TipoDeEvento,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                CondicionAntes = dto.CondicionAntes,
                CondicionDespues = dto.CondicionDespues,
                CantidadAfectada = dto.CantidadAfectada,
                AccionesTomadas = dto.AccionesTomadas,
                Recomendaciones = dto.Recomendaciones,
                RequiereSeguimiento = dto.RequiereSeguimiento,
                CreadoEn = DateTime.UtcNow,
                ReportadoPorIdUsuario = reportedByUserId,
                UrlsDeFotos = dto.UrlsDeFotos,
                EstaResuelto = dto.TipoDeEvento == TipoDeEventoDeUso.ConfirmacionDeUso || 
                           dto.TipoDeEvento == TipoDeEventoDeUso.FinalizacionDeUso ||
                           dto.TipoDeEvento == TipoDeEventoDeUso.VerificacionDeCalidad ||
                           dto.TipoDeEvento == TipoDeEventoDeUso.ProcesoDeDevolucion
            };

            // Si es un evento que se considera resuelto automáticamente
            if (log.EstaResuelto)
            {
                log.ResueltoEn = DateTime.UtcNow;
                log.ResueltoPorIdUsuario = reportedByUserId;
            }

            _db.RegistrosDeUsoDeRecurso.Add(log);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(log.Id) ?? throw new InvalidOperationException("Error al crear el registro de uso");
        }

        public async Task<RegistroDeUsoDeRecursoDto?> ResolveIncidentAsync(int logId, int resolvedByUserId, ResolverIncidenteDto dto)
        {
            var log = await _db.RegistrosDeUsoDeRecurso.FindAsync(logId);
            if (log == null) return null;

            if (log.EstaResuelto)
                throw new InvalidOperationException("Este incidente ya ha sido resuelto");

            // Verificar que el usuario existe
            var user = await _db.Usuarios.FindAsync(resolvedByUserId);
            if (user == null || !user.EstaActivo)
                throw new ArgumentException("El usuario especificado no existe o no está activo");

            log.EstaResuelto = true;
            log.ResueltoEn = DateTime.UtcNow;
            log.ResueltoPorIdUsuario = resolvedByUserId;
            log.NotasDeResolucion = dto.NotasDeResolucion;

            await _db.SaveChangesAsync();
            return await GetByIdAsync(logId);
        }

        public async Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetByAssignmentAsync(int assignmentId)
        {
            var logs = await _db.RegistrosDeUsoDeRecurso
                .Where(l => l.IdAsignacionDeRecurso == assignmentId)
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Recurso)
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Voluntario)
                .Include(l => l.ReportadoPor)
                .Include(l => l.ResueltoPor)
                .OrderByDescending(l => l.CreadoEn)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        public async Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetByResourceAsync(int resourceId)
        {
            var logs = await _db.RegistrosDeUsoDeRecurso
                .Where(l => l.AsignacionDeRecurso.IdRecurso == resourceId)
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Recurso)
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Voluntario)
                .Include(l => l.ReportadoPor)
                .Include(l => l.ResueltoPor)
                .OrderByDescending(l => l.CreadoEn)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        public async Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetByVolunteerAsync(int volunteerId)
        {
            var logs = await _db.RegistrosDeUsoDeRecurso
                .Where(l => l.AsignacionDeRecurso.IdVoluntario == volunteerId)
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Recurso)
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Voluntario)
                .Include(l => l.ReportadoPor)
                .Include(l => l.ResueltoPor)
                .OrderByDescending(l => l.CreadoEn)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        public async Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetIncidentsAsync()
        {
            var incidentTypes = new[]
            {
                TipoDeEventoDeUso.ReporteDeIncidente,
                TipoDeEventoDeUso.ReporteDeDanio,
                TipoDeEventoDeUso.ObservacionDeEstado
            };

            var logs = await _db.RegistrosDeUsoDeRecurso
                .Where(l => incidentTypes.Contains(l.TipoDeEvento))
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Recurso)
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Voluntario)
                .Include(l => l.ReportadoPor)
                .Include(l => l.ResueltoPor)
                .OrderByDescending(l => l.CreadoEn)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        public async Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetUnresolvedIncidentsAsync()
        {
            var incidentTypes = new[]
            {
                TipoDeEventoDeUso.ReporteDeIncidente,
                TipoDeEventoDeUso.ReporteDeDanio,
                TipoDeEventoDeUso.ObservacionDeEstado
            };

            var logs = await _db.RegistrosDeUsoDeRecurso
                .Where(l => incidentTypes.Contains(l.TipoDeEvento) && !l.EstaResuelto)
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Recurso)
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Voluntario)
                .Include(l => l.ReportadoPor)
                .Include(l => l.ResueltoPor)
                .OrderByDescending(l => l.CreadoEn)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        public async Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetByEventTypeAsync(TipoDeEventoDeUso eventType)
        {
            var logs = await _db.RegistrosDeUsoDeRecurso
                .Where(l => l.TipoDeEvento == eventType)
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Recurso)
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Voluntario)
                .Include(l => l.ReportadoPor)
                .Include(l => l.ResueltoPor)
                .OrderByDescending(l => l.CreadoEn)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        public async Task<object> GetUsageStatisticsAsync()
        {
            var totalLogs = await _db.RegistrosDeUsoDeRecurso.CountAsync();
            var incidentsCount = await _db.RegistrosDeUsoDeRecurso.CountAsync(l => 
                l.TipoDeEvento == TipoDeEventoDeUso.ReporteDeIncidente || 
                l.TipoDeEvento == TipoDeEventoDeUso.ReporteDeDanio);
            var unresolvedIncidents = await _db.RegistrosDeUsoDeRecurso.CountAsync(l => 
                (l.TipoDeEvento == TipoDeEventoDeUso.ReporteDeIncidente || 
                 l.TipoDeEvento == TipoDeEventoDeUso.ReporteDeDanio) && !l.EstaResuelto);
            var confirmationsCount = await _db.RegistrosDeUsoDeRecurso.CountAsync(l => 
                l.TipoDeEvento == TipoDeEventoDeUso.ConfirmacionDeUso);
            var completionsCount = await _db.RegistrosDeUsoDeRecurso.CountAsync(l => 
                l.TipoDeEvento == TipoDeEventoDeUso.FinalizacionDeUso);

            var eventTypeStats = await _db.RegistrosDeUsoDeRecurso
                .GroupBy(l => l.TipoDeEvento)
                .Select(g => new
                {
                    EventType = g.Key.ToString(),
                    Count = g.Count(),
                    ResolvedCount = g.Count(l => l.EstaResuelto),
                    UnresolvedCount = g.Count(l => !l.EstaResuelto)
                })
                .ToListAsync();

            var last30Days = DateTime.UtcNow.AddDays(-30);
            var recentActivities = await _db.RegistrosDeUsoDeRecurso
                .Where(l => l.CreadoEn >= last30Days)
                .CountAsync();

            return new
            {
                TotalLogs = totalLogs,
                IncidentsCount = incidentsCount,
                UnresolvedIncidents = unresolvedIncidents,
                ConfirmationsCount = confirmationsCount,
                CompletionsCount = completionsCount,
                EventTypeStatistics = eventTypeStats,
                RecentActivities = recentActivities
            };
        }

        public async Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetRecentActivitiesAsync(int take = 20)
        {
            var logs = await _db.RegistrosDeUsoDeRecurso
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Recurso)
                .Include(l => l.AsignacionDeRecurso)
                    .ThenInclude(a => a.Voluntario)
                .Include(l => l.ReportadoPor)
                .Include(l => l.ResueltoPor)
                .OrderByDescending(l => l.CreadoEn)
                .Take(take)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        private static RegistroDeUsoDeRecursoDto MapToDto(RegistroDeUsoDeRecurso log)
        {
            return new RegistroDeUsoDeRecursoDto
            {
                Id = log.Id,
                AsignacionDeRecursoId = log.IdAsignacionDeRecurso,
                NombreRecurso = log.AsignacionDeRecurso.Recurso.Nombre,
                NombreVoluntario = $"{log.AsignacionDeRecurso.Voluntario.PrimerNombre} {log.AsignacionDeRecurso.Voluntario.Apellido}",
                TipoDeEvento = log.TipoDeEvento,
                Titulo = log.Titulo,
                Descripcion = log.Descripcion,
                CondicionAntes = log.CondicionAntes,
                CondicionDespues = log.CondicionDespues,
                CantidadAfectada = log.CantidadAfectada,
                AccionesTomadas = log.AccionesTomadas,
                Recomendaciones = log.Recomendaciones,
                RequiereSeguimiento = log.RequiereSeguimiento,
                CreadoEn = log.CreadoEn,
                ReportadoPorIdUsuario = log.ReportadoPorIdUsuario,
                ReportadoPorNombreUsuario = $"{log.ReportadoPor.PrimerNombre} {log.ReportadoPor.Apellido}",
                UrlsDeFotos = log.UrlsDeFotos,
                EstaResuelto = log.EstaResuelto,
                ResueltoEn = log.ResueltoEn,
                ResueltoPorIdUsuario = log.ResueltoPorIdUsuario,
                ResueltoPorNombreUsuario = log.ResueltoPor != null ? $"{log.ResueltoPor.PrimerNombre} {log.ResueltoPor.Apellido}" : null,
                NotasDeResolucion = log.NotasDeResolucion
            };
        }
    }
}
