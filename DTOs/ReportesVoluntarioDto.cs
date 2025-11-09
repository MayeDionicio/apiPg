using System.ComponentModel.DataAnnotations;

namespace ApiPG.DTOs
{
    /// <summary>
    /// DTOs para reportes específicos de voluntarios
    /// Los voluntarios solo ven información de su nivel asignado
    /// </summary>
    
    public class ReporteVoluntarioDto
    {
        public InfoNivelVoluntarioDto MiNivel { get; set; } = new();
        public EstadisticasAsistenciaVoluntarioDto Asistencia { get; set; } = new();
        public List<ParticipanteDelNivelDto> Participantes { get; set; } = new();
        public DateTime FechaGeneracion { get; set; }
    }

    public class InfoNivelVoluntarioDto
    {
        public int IdNivel { get; set; }
        public string NombreNivel { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal EdadMinima { get; set; }
        public decimal EdadMaxima { get; set; }
        public int TotalParticipantes { get; set; }
        public int ParticipantesActivos { get; set; }
    }

    public class EstadisticasAsistenciaVoluntarioDto
    {
        public int TotalRegistrosHoy { get; set; }
        public int PresentesHoy { get; set; }
        public int AusentesHoy { get; set; }
        public decimal PorcentajeAsistenciaHoy { get; set; }
        
        public int TotalRegistrosEstaSemana { get; set; }
        public int PresentesEstaSemana { get; set; }
        public decimal PorcentajeAsistenciaEstaSemana { get; set; }
        
        public int TotalRegistrosEsteMes { get; set; }
        public int PresentesEsteMes { get; set; }
        public decimal PorcentajeAsistenciaEsteMes { get; set; }
        
        public List<AsistenciaDiariaVoluntarioDto> UltimosDias { get; set; } = new();
    }

    public class AsistenciaDiariaVoluntarioDto
    {
        public DateTime Fecha { get; set; }
        public int TotalRegistros { get; set; }
        public int Presentes { get; set; }
        public int Ausentes { get; set; }
        public decimal PorcentajeAsistencia { get; set; }
    }

    public class ParticipanteDelNivelDto
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public int Edad { get; set; }
        public bool EstaActivo { get; set; }
        
        // Estadísticas de asistencia del participante
        public int TotalAsistencias { get; set; }
        public int TotalPresencias { get; set; }
        public int TotalAusencias { get; set; }
        public decimal PorcentajeAsistencia { get; set; }
        public DateTime? UltimaAsistencia { get; set; }
    }

    public class AsistenciaDetalladaVoluntarioDto
    {
        public int IdAsistencia { get; set; }
        public int IdParticipante { get; set; }
        public string NombreParticipante { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public bool Presente { get; set; }
        public string? Observaciones { get; set; }
        public DateTime CreadoEn { get; set; }
    }

    public class FiltrosReporteVoluntarioDto
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? IdParticipante { get; set; }
    }
}
