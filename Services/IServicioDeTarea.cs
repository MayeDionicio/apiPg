using ApiPG.Models;
using ApiPG.DTOs;

namespace ApiPG.Services
{
    public interface IServicioDeTarea
    {
        // Coordinador - CRUD de tareas
        Task<TareaDto> CrearTareaAsync(CrearTareaDto dto, int coordinadorId);
        Task<TareaDto?> ObtenerTareaPorIdAsync(int id);
        Task<IEnumerable<TareaDto>> ObtenerTodasLasTareasAsync();
        Task<IEnumerable<TareaDto>> ObtenerTareasPorCoordinadorAsync(int coordinadorId);
        Task<TareaDto> ActualizarTareaAsync(int id, ActualizarTareaDto dto, int coordinadorId);
        Task<bool> EliminarTareaAsync(int id, int coordinadorId);

        // Voluntario - Ver sus tareas asignadas
        Task<IEnumerable<TareaDto>> ObtenerTareasDeVoluntarioAsync(int voluntarioId);
        Task<TareaDto> ActualizarEstadoTareaAsync(int tareaId, int voluntarioId, ActualizarEstadoTareaDto dto);

        // Coordinador - Aprobar/Rechazar tareas
        Task<TareaDto> RevisarTareaAsync(int tareaId, RevisarTareaDto dto, int coordinadorId);

        // Estadísticas
        Task<EstadisticasTareasDto> ObtenerEstadisticasAsync(int? coordinadorId = null, int? voluntarioId = null);
    }
}
