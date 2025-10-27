using ApiPG.DTOs;

namespace ApiPG.Services
{
    public interface IServicioDeActividad
    {
        Task<ActividadDto> CrearActividadAsync(CrearActividadDto dto, int coordinadorId);
        Task<ActividadDto?> ObtenerActividadPorIdAsync(int id);
        Task<IEnumerable<ActividadDto>> ObtenerTodasLasActividadesAsync();
        Task<IEnumerable<ActividadDto>> ObtenerActividadesPorCoordinadorAsync(int coordinadorId);
        Task<IEnumerable<ActividadDto>> ObtenerActividadesDeVoluntarioAsync(int voluntarioId);
        Task<IEnumerable<ActividadDto>> ObtenerActividadesPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<ActividadDto> ActualizarActividadAsync(int id, ActualizarActividadDto dto, int coordinadorId);
        Task<bool> EliminarActividadAsync(int id, int coordinadorId);
        Task<EstadisticasActividadesDto> ObtenerEstadisticasAsync(int? coordinadorId = null);
    }
}
