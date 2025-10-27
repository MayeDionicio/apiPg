using ApiPG.Models;

namespace ApiPG.DTOs
{
    // DTO para crear una nueva tarea
    public class CrearTareaDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public List<int> VoluntariosIds { get; set; } = new List<int>();
        public string? NotasCoordinador { get; set; }
    }

    // DTO para actualizar una tarea
    public class ActualizarTareaDto
    {
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public List<int>? VoluntariosIds { get; set; }
        public string? NotasCoordinador { get; set; }
    }

    // DTO para que el voluntario actualice el estado
    public class ActualizarEstadoTareaDto
    {
        public EstadoTarea Estado { get; set; }
        public string? NotasVoluntario { get; set; }
    }

    // DTO para que el coordinador apruebe/rechace
    public class RevisarTareaDto
    {
        public bool Aprobar { get; set; }
        public string? RazonRechazo { get; set; }
    }

    // DTO de respuesta con información del voluntario asignado
    public class AsignacionTareaDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NombreVoluntario { get; set; } = string.Empty;
        public string EmailVoluntario { get; set; } = string.Empty;
        public EstadoTarea EstadoVoluntario { get; set; }
        public DateTime AsignadoEn { get; set; }
        public DateTime? VistaEn { get; set; }
        public DateTime? IniciadaEn { get; set; }
        public DateTime? CompletadaEn { get; set; }
    }

    // DTO de respuesta completo de tarea
    public class TareaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int CreadoPorIdUsuario { get; set; }
        public string NombreCoordinador { get; set; } = string.Empty;
        public EstadoTarea Estado { get; set; }
        public string EstadoTexto { get; set; } = string.Empty;
        public List<AsignacionTareaDto> Voluntarios { get; set; } = new List<AsignacionTareaDto>();
        public string? NotasCoordinador { get; set; }
        public string? NotasVoluntario { get; set; }
        public DateTime CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public DateTime? FechaRevision { get; set; }
        public string? RazonRechazo { get; set; }
        public bool EstaActivo { get; set; }
    }

    // DTO para estadísticas de tareas
    public class EstadisticasTareasDto
    {
        public int TotalTareas { get; set; }
        public int TareasPendientes { get; set; }
        public int TareasEnProceso { get; set; }
        public int TareasCompletadas { get; set; }
        public int TareasAprobadas { get; set; }
        public int TareasRechazadas { get; set; }
        public int TareasVencidas { get; set; }
        public double PorcentajeCompletadas { get; set; }
    }
}
