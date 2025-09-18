using ApiPG.DTOs;
using ApiPG.Models;

namespace ApiPG.Services
{
    public interface IResourceService
    {
        // CRUD básico para Resources
        Task<IEnumerable<ResourceDto>> GetAllAsync();
        Task<IEnumerable<ResourceDto>> GetActiveAsync();
        Task<ResourceDto?> GetByIdAsync(int id);
        Task<ResourceDto> CreateAsync(CreateResourceDto dto);
        Task<ResourceDto?> UpdateAsync(int id, UpdateResourceDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> ActivateAsync(int id);

        // Operaciones específicas de recursos
        Task<IEnumerable<ResourceDto>> GetByCategoryAsync(string category);
        Task<IEnumerable<ResourceDto>> GetAvailableAsync();
        Task<IEnumerable<ResourceDto>> GetLowStockAsync(int threshold = 5);
        Task<bool> UpdateQuantityAsync(int resourceId, int newQuantity);
        Task<bool> ReserveQuantityAsync(int resourceId, int quantity);
        Task<bool> ReleaseQuantityAsync(int resourceId, int quantity);

        // Estadísticas
        Task<object> GetResourceStatisticsAsync();
        Task<IEnumerable<string>> GetCategoriesAsync();
    }

    public interface IResourceAssignmentService
    {
        // CRUD básico para ResourceAssignments
        Task<IEnumerable<ResourceAssignmentDto>> GetAllAsync();
        Task<ResourceAssignmentDto?> GetByIdAsync(int id);
        Task<ResourceAssignmentDto> CreateAsync(CreateResourceAssignmentDto dto, int assignedByUserId);
        Task<ResourceAssignmentDto?> UpdateAsync(int id, UpdateResourceAssignmentDto dto);
        Task<bool> SoftDeleteAsync(int id);

        // Operaciones específicas de asignaciones
        Task<ResourceAssignmentDto?> ConfirmAssignmentAsync(int assignmentId, int volunteerId, ConfirmAssignmentDto dto);
        Task<ResourceAssignmentDto?> StartUseAsync(int assignmentId, int volunteerId);
        Task<ResourceAssignmentDto?> ReturnResourceAsync(int assignmentId, int volunteerId);
        Task<ResourceAssignmentDto?> CancelAssignmentAsync(int assignmentId, int userId);

        // Consultas específicas
        Task<IEnumerable<ResourceAssignmentDto>> GetByVolunteerAsync(int volunteerId);
        Task<IEnumerable<ResourceAssignmentDto>> GetByResourceAsync(int resourceId);
        Task<IEnumerable<ResourceAssignmentDto>> GetPendingAssignmentsAsync();
        Task<IEnumerable<ResourceAssignmentDto>> GetOverdueReturnsAsync();
        Task<IEnumerable<ResourceAssignmentDto>> GetActiveAssignmentsAsync();

        // Estadísticas
        Task<object> GetAssignmentStatisticsAsync();
    }

    public interface IResourceUsageLogService
    {
        // CRUD básico para ResourceUsageLogs
        Task<IEnumerable<ResourceUsageLogDto>> GetAllAsync();
        Task<ResourceUsageLogDto?> GetByIdAsync(int id);
        Task<ResourceUsageLogDto> CreateAsync(CreateResourceUsageLogDto dto, int reportedByUserId);
        Task<ResourceUsageLogDto?> ResolveIncidentAsync(int logId, int resolvedByUserId, ResolveIncidentDto dto);

        // Consultas específicas
        Task<IEnumerable<ResourceUsageLogDto>> GetByAssignmentAsync(int assignmentId);
        Task<IEnumerable<ResourceUsageLogDto>> GetByResourceAsync(int resourceId);
        Task<IEnumerable<ResourceUsageLogDto>> GetByVolunteerAsync(int volunteerId);
        Task<IEnumerable<ResourceUsageLogDto>> GetIncidentsAsync();
        Task<IEnumerable<ResourceUsageLogDto>> GetUnresolvedIncidentsAsync();
        Task<IEnumerable<ResourceUsageLogDto>> GetByEventTypeAsync(UsageEventType eventType);

        // Estadísticas
        Task<object> GetUsageStatisticsAsync();
        Task<IEnumerable<ResourceUsageLogDto>> GetRecentActivitiesAsync(int take = 20);
    }
}