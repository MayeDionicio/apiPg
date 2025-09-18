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

        public async Task<IEnumerable<ResourceUsageLogDto>> GetAllAsync()
        {
            var logs = await _db.ResourceUsageLogs
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Resource)
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Volunteer)
                .Include(l => l.ReportedByUser)
                .Include(l => l.ResolvedByUser)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        public async Task<ResourceUsageLogDto?> GetByIdAsync(int id)
        {
            var log = await _db.ResourceUsageLogs
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Resource)
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Volunteer)
                .Include(l => l.ReportedByUser)
                .Include(l => l.ResolvedByUser)
                .FirstOrDefaultAsync(l => l.Id == id);

            return log != null ? MapToDto(log) : null;
        }

        public async Task<ResourceUsageLogDto> CreateAsync(CreateResourceUsageLogDto dto, int reportedByUserId)
        {
            // Verificar que la asignación existe
            var assignment = await _db.ResourceAssignments.FindAsync(dto.ResourceAssignmentId);
            if (assignment == null)
                throw new ArgumentException("La asignación de recurso especificada no existe");

            // Verificar que el usuario existe
            var user = await _db.Users.FindAsync(reportedByUserId);
            if (user == null || !user.IsActive)
                throw new ArgumentException("El usuario especificado no existe o no está activo");

            var log = new ResourceUsageLog
            {
                ResourceAssignmentId = dto.ResourceAssignmentId,
                EventType = dto.EventType,
                Title = dto.Title,
                Description = dto.Description,
                ConditionBefore = dto.ConditionBefore,
                ConditionAfter = dto.ConditionAfter,
                QuantityAffected = dto.QuantityAffected,
                ActionsTaken = dto.ActionsTaken,
                Recommendations = dto.Recommendations,
                RequiresFollowUp = dto.RequiresFollowUp,
                CreatedAt = DateTime.UtcNow,
                ReportedByUserId = reportedByUserId,
                PhotoUrls = dto.PhotoUrls,
                IsResolved = dto.EventType == UsageEventType.ConfirmationOfUse || 
                           dto.EventType == UsageEventType.UsageCompletion ||
                           dto.EventType == UsageEventType.QualityCheck ||
                           dto.EventType == UsageEventType.ReturnProcess
            };

            // Si es un evento que se considera resuelto automáticamente
            if (log.IsResolved)
            {
                log.ResolvedAt = DateTime.UtcNow;
                log.ResolvedByUserId = reportedByUserId;
            }

            _db.ResourceUsageLogs.Add(log);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(log.Id) ?? throw new InvalidOperationException("Error al crear el registro de uso");
        }

        public async Task<ResourceUsageLogDto?> ResolveIncidentAsync(int logId, int resolvedByUserId, ResolveIncidentDto dto)
        {
            var log = await _db.ResourceUsageLogs.FindAsync(logId);
            if (log == null) return null;

            if (log.IsResolved)
                throw new InvalidOperationException("Este incidente ya ha sido resuelto");

            // Verificar que el usuario existe
            var user = await _db.Users.FindAsync(resolvedByUserId);
            if (user == null || !user.IsActive)
                throw new ArgumentException("El usuario especificado no existe o no está activo");

            log.IsResolved = true;
            log.ResolvedAt = DateTime.UtcNow;
            log.ResolvedByUserId = resolvedByUserId;
            log.ResolutionNotes = dto.ResolutionNotes;

            await _db.SaveChangesAsync();
            return await GetByIdAsync(logId);
        }

        public async Task<IEnumerable<ResourceUsageLogDto>> GetByAssignmentAsync(int assignmentId)
        {
            var logs = await _db.ResourceUsageLogs
                .Where(l => l.ResourceAssignmentId == assignmentId)
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Resource)
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Volunteer)
                .Include(l => l.ReportedByUser)
                .Include(l => l.ResolvedByUser)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        public async Task<IEnumerable<ResourceUsageLogDto>> GetByResourceAsync(int resourceId)
        {
            var logs = await _db.ResourceUsageLogs
                .Where(l => l.ResourceAssignment.ResourceId == resourceId)
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Resource)
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Volunteer)
                .Include(l => l.ReportedByUser)
                .Include(l => l.ResolvedByUser)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        public async Task<IEnumerable<ResourceUsageLogDto>> GetByVolunteerAsync(int volunteerId)
        {
            var logs = await _db.ResourceUsageLogs
                .Where(l => l.ResourceAssignment.VolunteerId == volunteerId)
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Resource)
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Volunteer)
                .Include(l => l.ReportedByUser)
                .Include(l => l.ResolvedByUser)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        public async Task<IEnumerable<ResourceUsageLogDto>> GetIncidentsAsync()
        {
            var incidentTypes = new[]
            {
                UsageEventType.IncidentReport,
                UsageEventType.DamageReport,
                UsageEventType.StateObservation
            };

            var logs = await _db.ResourceUsageLogs
                .Where(l => incidentTypes.Contains(l.EventType))
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Resource)
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Volunteer)
                .Include(l => l.ReportedByUser)
                .Include(l => l.ResolvedByUser)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        public async Task<IEnumerable<ResourceUsageLogDto>> GetUnresolvedIncidentsAsync()
        {
            var incidentTypes = new[]
            {
                UsageEventType.IncidentReport,
                UsageEventType.DamageReport,
                UsageEventType.StateObservation
            };

            var logs = await _db.ResourceUsageLogs
                .Where(l => incidentTypes.Contains(l.EventType) && !l.IsResolved)
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Resource)
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Volunteer)
                .Include(l => l.ReportedByUser)
                .Include(l => l.ResolvedByUser)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        public async Task<IEnumerable<ResourceUsageLogDto>> GetByEventTypeAsync(UsageEventType eventType)
        {
            var logs = await _db.ResourceUsageLogs
                .Where(l => l.EventType == eventType)
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Resource)
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Volunteer)
                .Include(l => l.ReportedByUser)
                .Include(l => l.ResolvedByUser)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        public async Task<object> GetUsageStatisticsAsync()
        {
            var totalLogs = await _db.ResourceUsageLogs.CountAsync();
            var incidentsCount = await _db.ResourceUsageLogs.CountAsync(l => 
                l.EventType == UsageEventType.IncidentReport || 
                l.EventType == UsageEventType.DamageReport);
            var unresolvedIncidents = await _db.ResourceUsageLogs.CountAsync(l => 
                (l.EventType == UsageEventType.IncidentReport || 
                 l.EventType == UsageEventType.DamageReport) && !l.IsResolved);
            var confirmationsCount = await _db.ResourceUsageLogs.CountAsync(l => 
                l.EventType == UsageEventType.ConfirmationOfUse);
            var completionsCount = await _db.ResourceUsageLogs.CountAsync(l => 
                l.EventType == UsageEventType.UsageCompletion);

            var eventTypeStats = await _db.ResourceUsageLogs
                .GroupBy(l => l.EventType)
                .Select(g => new
                {
                    EventType = g.Key.ToString(),
                    Count = g.Count(),
                    ResolvedCount = g.Count(l => l.IsResolved),
                    UnresolvedCount = g.Count(l => !l.IsResolved)
                })
                .ToListAsync();

            var last30Days = DateTime.UtcNow.AddDays(-30);
            var recentActivities = await _db.ResourceUsageLogs
                .Where(l => l.CreatedAt >= last30Days)
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

        public async Task<IEnumerable<ResourceUsageLogDto>> GetRecentActivitiesAsync(int take = 20)
        {
            var logs = await _db.ResourceUsageLogs
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Resource)
                .Include(l => l.ResourceAssignment)
                    .ThenInclude(a => a.Volunteer)
                .Include(l => l.ReportedByUser)
                .Include(l => l.ResolvedByUser)
                .OrderByDescending(l => l.CreatedAt)
                .Take(take)
                .ToListAsync();

            return logs.Select(MapToDto);
        }

        private static ResourceUsageLogDto MapToDto(ResourceUsageLog log)
        {
            return new ResourceUsageLogDto
            {
                Id = log.Id,
                ResourceAssignmentId = log.ResourceAssignmentId,
                ResourceName = log.ResourceAssignment.Resource.Name,
                VolunteerName = $"{log.ResourceAssignment.Volunteer.FirstName} {log.ResourceAssignment.Volunteer.LastName}",
                EventType = log.EventType,
                Title = log.Title,
                Description = log.Description,
                ConditionBefore = log.ConditionBefore,
                ConditionAfter = log.ConditionAfter,
                QuantityAffected = log.QuantityAffected,
                ActionsTaken = log.ActionsTaken,
                Recommendations = log.Recommendations,
                RequiresFollowUp = log.RequiresFollowUp,
                CreatedAt = log.CreatedAt,
                ReportedByUserId = log.ReportedByUserId,
                ReportedByUserName = $"{log.ReportedByUser.FirstName} {log.ReportedByUser.LastName}",
                PhotoUrls = log.PhotoUrls,
                IsResolved = log.IsResolved,
                ResolvedAt = log.ResolvedAt,
                ResolvedByUserId = log.ResolvedByUserId,
                ResolvedByUserName = log.ResolvedByUser != null ? $"{log.ResolvedByUser.FirstName} {log.ResolvedByUser.LastName}" : null,
                ResolutionNotes = log.ResolutionNotes
            };
        }
    }
}