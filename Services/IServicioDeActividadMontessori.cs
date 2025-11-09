using ApiPG.DTOs;

namespace ApiPG.Services
{
    public interface IServicioDeActividadMontessori
    {
        // Actividades Montessori
        Task<ActividadMontessoriDto> CrearActividadAsync(CrearActividadMontessoriDto dto, int voluntarioId);
        Task<ActividadMontessoriDto?> ObtenerActividadPorIdAsync(int id);
        Task<IEnumerable<ActividadMontessoriDto>> ObtenerTodasLasActividadesAsync();
        Task<IEnumerable<ActividadMontessoriDto>> ObtenerActividadesPorVoluntarioAsync(int voluntarioId);
        Task<IEnumerable<ActividadMontessoriDto>> ObtenerActividadesPorAreaAsync(string area);
        Task<IEnumerable<ActividadMontessoriDto>> ObtenerActividadesParaParticipanteAsync(int participanteId);
        Task<ActividadMontessoriDto> ActualizarActividadAsync(int id, ActualizarActividadMontessoriDto dto, int voluntarioId);
        Task<bool> EliminarActividadAsync(int id, int voluntarioId);
        Task<EstadisticasActividadesMontessoriDto> ObtenerEstadisticasAsync(int? voluntarioId = null);
        
        // Logros
        Task<LogroMontessoriDto> CrearLogroAsync(CrearLogroMontessoriDto dto);
        Task<LogroMontessoriDto> MarcarLogroComoObtenidoAsync(int logroId);
        Task<IEnumerable<LogroMontessoriDto>> ObtenerLogrosPorActividadAsync(int actividadId);
        Task<IEnumerable<LogroMontessoriDto>> ObtenerLogrosPorUsuarioAsync(int usuarioId);
        Task<bool> EliminarLogroAsync(int logroId);
    }
}
