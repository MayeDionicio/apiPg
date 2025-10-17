using ApiPG.DTOs;
using ApiPG.Models;

namespace ApiPG.Services
{
    public interface IDevocionalService
    {
        // CRUD básico
        Task<IEnumerable<DevocionalDto>> GetAllAsync();
        Task<DevocionalDto?> GetByIdAsync(int id);
        Task<DevocionalDto> CreateAsync(CrearDevocionalDto createDto, int createdByUserId);
        Task<DevocionalDto?> UpdateAsync(int id, ActualizarDevocionalDto updateDto, int updatedByUserId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ActivateAsync(int id);

        // Métodos específicos para el wizard
        Task<DevocionalDto> SaveStep1Async(DevocionalPaso1Dto step1, int userId);
        Task<DevocionalDto> SaveStep2Async(int devocionalId, DevocionalPaso2Dto step2, int userId);
        Task<DevocionalDto> SaveStep3Async(int devocionalId, DevocionalPaso3Dto step3, int userId);
        Task<DevocionalDto> SaveDraftAsync(GuardarBorradorDto draftDto, int userId);
        Task<DevocionalDto> FinalizeDevocionalAsync(int devocionalId, int userId);

        // Filtros y consultas
        Task<IEnumerable<DevocionalDto>> GetByEstadoAsync(DevocionalEstado estado);
        Task<IEnumerable<DevocionalDto>> GetByVoluntarioAsync(int voluntarioId);
        Task<IEnumerable<DevocionalDto>> GetByFechaAsync(DateTime fecha);
        Task<IEnumerable<DevocionalDto>> GetByRangoFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<DevocionalDto>> GetBorradoresAsync();
        Task<IEnumerable<DevocionalDto>> GetProgramadosAsync();
        Task<IEnumerable<DevocionalDto>> GetProximosAsync(int dias = 7);
        Task<IEnumerable<DevocionalDto>> GetRecentesAsync(int take = 10);

        // Cambios de estado
        Task<DevocionalDto?> CambiarEstadoAsync(int id, DevocionalEstado nuevoEstado, int userId);
        Task<DevocionalDto?> ProgrimarDevocionalAsync(int id, int userId);
        Task<DevocionalDto?> IniciarDevocionalAsync(int id, int userId);
        Task<DevocionalDto?> CompletarDevocionalAsync(int id, int userId);
        Task<DevocionalDto?> CancelarDevocionalAsync(int id, int userId);

        // Utilidades
        Task<DevocionalVistaPreviaDto?> GetPreviewAsync(int id);
        Task<IEnumerable<string>> GetVoluntariosDisponiblesAsync();
        Task<DevocionalEstadisticasDto> GetEstadisticasAsync();

        // Búsqueda
        Task<IEnumerable<DevocionalDto>> SearchAsync(string searchTerm);
        Task<IEnumerable<DevocionalDto>> GetByCreatedByUserAsync(int userId);
    }
}
