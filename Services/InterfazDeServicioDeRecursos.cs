using ApiPG.DTOs;
using ApiPG.Models;

namespace ApiPG.Services
{
    public interface IResourceService
    {
        // CRUD básico para Resources
        Task<IEnumerable<RecursoDto>> GetAllAsync();
        Task<IEnumerable<RecursoDto>> GetActiveAsync();
        Task<RecursoDto?> GetByIdAsync(int id);
        Task<RecursoDto> CreateAsync(CrearRecursoDto dto);
        Task<RecursoDto?> UpdateAsync(int id, ActualizarRecursoDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> ActivateAsync(int id);

        // Operaciones específicas de recursos
        Task<IEnumerable<RecursoDto>> GetByCategoryAsync(string category);
        Task<IEnumerable<RecursoDto>> GetAvailableAsync();
        Task<IEnumerable<RecursoDto>> GetLowStockAsync(int threshold = 5);
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
        Task<IEnumerable<AsignacionDeRecursoDto>> GetAllAsync();
        Task<AsignacionDeRecursoDto?> GetByIdAsync(int id);
        Task<AsignacionDeRecursoDto> CreateAsync(CrearAsignacionDeRecursoDto dto, int assignedByUserId);
        Task<AsignacionDeRecursoDto?> UpdateAsync(int id, ActualizarAsignacionDeRecursoDto dto);
        Task<bool> SoftDeleteAsync(int id);

        // Operaciones específicas de asignaciones
        Task<AsignacionDeRecursoDto?> ConfirmAssignmentAsync(int assignmentId, int volunteerId, ConfirmarAsignacionDto dto);
        Task<AsignacionDeRecursoDto?> StartUseAsync(int assignmentId, int volunteerId);
        Task<AsignacionDeRecursoDto?> ReturnResourceAsync(int assignmentId, int volunteerId);
        Task<AsignacionDeRecursoDto?> CancelAssignmentAsync(int assignmentId, int userId);

        // Consultas específicas
        Task<IEnumerable<AsignacionDeRecursoDto>> GetByVolunteerAsync(int volunteerId);
        Task<IEnumerable<AsignacionDeRecursoDto>> GetByResourceAsync(int resourceId);
        Task<IEnumerable<AsignacionDeRecursoDto>> GetPendingAssignmentsAsync();
        Task<IEnumerable<AsignacionDeRecursoDto>> GetOverdueReturnsAsync();
        Task<IEnumerable<AsignacionDeRecursoDto>> GetActiveAssignmentsAsync();

        // Estadísticas
        Task<object> GetAssignmentStatisticsAsync();
    }

    public interface IResourceUsageLogService
    {
        // CRUD básico para ResourceUsageLogs
        Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetAllAsync();
        Task<RegistroDeUsoDeRecursoDto?> GetByIdAsync(int id);
        Task<RegistroDeUsoDeRecursoDto> CreateAsync(CrearRegistroDeUsoDeRecursoDto dto, int reportedByUserId);
        Task<RegistroDeUsoDeRecursoDto?> ResolveIncidentAsync(int logId, int resolvedByUserId, ResolverIncidenteDto dto);

        // Consultas específicas
        Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetByAssignmentAsync(int assignmentId);
        Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetByResourceAsync(int resourceId);
        Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetByVolunteerAsync(int volunteerId);
        Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetIncidentsAsync();
        Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetUnresolvedIncidentsAsync();
        Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetByEventTypeAsync(TipoDeEventoDeUso eventType);

        // Estadísticas
        Task<object> GetUsageStatisticsAsync();
        Task<IEnumerable<RegistroDeUsoDeRecursoDto>> GetRecentActivitiesAsync(int take = 20);
    }
}
