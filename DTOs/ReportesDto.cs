using System.ComponentModel.DataAnnotations;

namespace ApiPG.DTOs
{
    /// <summary>
    /// DTOs para reportes generales del sistema
    /// </summary>
    
    public class ReporteGeneralDto
    {
        public ResumenUsuariosDto Usuarios { get; set; } = new();
        public ResumenActividadesDto Actividades { get; set; } = new();
        public ResumenNivelesDto Niveles { get; set; } = new();
        public ResumenAsistenciaDto Asistencia { get; set; } = new();
        public ResumenTareasDto Tareas { get; set; } = new();
        public DateTime FechaGeneracion { get; set; }
    }

    public class ResumenUsuariosDto
    {
        public int TotalUsuarios { get; set; }
        public int TotalCoordinadores { get; set; }
        public int TotalVoluntarios { get; set; }
        public int TotalParticipantes { get; set; }
        public int UsuariosActivos { get; set; }
        public int UsuariosInactivos { get; set; }
        public List<UsuariosPorRolDto> DesglosePorRol { get; set; } = new();
    }

    public class UsuariosPorRolDto
    {
        public string NombreRol { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public int Activos { get; set; }
        public int Inactivos { get; set; }
        public decimal PorcentajeActivos { get; set; }
    }

    public class ResumenActividadesDto
    {
        public int TotalActividades { get; set; }
        public int ActividadesRegulares { get; set; }
        public int ActividadesMontessori { get; set; }
        public int ActividadesActivas { get; set; }
        public int ActividadesInactivas { get; set; }
        public int LogrosMontessoriObtenidos { get; set; }
        public int LogrosMontessoriTotales { get; set; }
        public decimal PorcentajeLogrosObtenidos { get; set; }
        public List<ActividadesPorAreaDto> ActividadesPorArea { get; set; } = new();
    }

    public class ActividadesPorAreaDto
    {
        public string AreaPedagogica { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class ResumenNivelesDto
    {
        public int TotalNiveles { get; set; }
        public int NivelesActivos { get; set; }
        public int TotalParticipantesAsignados { get; set; }
        public decimal PromedioParticipantesPorNivel { get; set; }
        public List<NivelConParticipantesDto> DesglosePorNivel { get; set; } = new();
    }

    public class NivelConParticipantesDto
    {
        public int IdNivel { get; set; }
        public string NombreNivel { get; set; } = string.Empty;
        public string? NombreVoluntario { get; set; }
        public decimal EdadMinima { get; set; }
        public decimal EdadMaxima { get; set; }
        public int CantidadParticipantes { get; set; }
    }

    public class ResumenAsistenciaDto
    {
        public int TotalRegistrosHoy { get; set; }
        public int TotalRegistrosEsteMes { get; set; }
        public int TotalRegistrosEsteAnio { get; set; }
        public decimal PorcentajeAsistenciaPromedio { get; set; }
        public List<AsistenciaPorDiaDto> AsistenciaUltimos7Dias { get; set; } = new();
    }

    public class AsistenciaPorDiaDto
    {
        public DateTime Fecha { get; set; }
        public int CantidadPresentes { get; set; }
        public int CantidadAusentes { get; set; }
        public int CantidadTardanzas { get; set; }
        public int Total { get; set; }
    }

    public class ResumenTareasDto
    {
        public int TotalTareas { get; set; }
        public int TareasPendientes { get; set; }
        public int TareasEnProceso { get; set; }
        public int TareasCompletadas { get; set; }
        public int TareasAprobadas { get; set; }
        public int TareasRechazadas { get; set; }
        public decimal PorcentajeCompletadas { get; set; }
        public List<TareasPorVoluntarioDto> TareasPorVoluntario { get; set; } = new();
    }

    public class TareasPorVoluntarioDto
    {
        public int IdVoluntario { get; set; }
        public string NombreVoluntario { get; set; } = string.Empty;
        public int TareasAsignadas { get; set; }
        public int TareasCompletadas { get; set; }
        public decimal PorcentajeCompletadas { get; set; }
    }

    public class FiltrosReporteDto
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? IdNivel { get; set; }
        public int? IdVoluntario { get; set; }
        public bool IncluirInactivos { get; set; } = false;
    }

    public class ReporteDetalladoDto
    {
        public string TipoReporte { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public Dictionary<string, object> Datos { get; set; } = new();
        public List<string> Observaciones { get; set; } = new();
    }
}
