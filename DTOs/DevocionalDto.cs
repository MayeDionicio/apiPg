using ApiPG.Models;
using System.ComponentModel.DataAnnotations;

namespace ApiPG.DTOs
{
    public class DevocionalDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public DateTime? FechaProgramada { get; set; }
        public int? VoluntarioAsignadoId { get; set; }
        public string? VoluntarioAsignado { get; set; }
        public string Pasaje { get; set; } = string.Empty;
        public string? TextoClave { get; set; }
        public string? Objetivo { get; set; }
        public string? Idea { get; set; }
        public List<PuntoPrincipal>? PuntosPrincipales { get; set; }
        public string? Aplicacion { get; set; }
        public string? Reto { get; set; }
        public string? Oracion { get; set; }
        public string? Recursos { get; set; }
        public DevocionalEstado Estado { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public bool IsActive { get; set; }
        
        // Campos calculados
        public string FechaFormateada => FechaProgramada?.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES")) ?? "";
        public string EstadoTexto => Estado.ToString();
        public bool EsBorrador => Estado == DevocionalEstado.Borrador;
        public bool EsProgramado => Estado == DevocionalEstado.Programado;
        public bool TienePuntosPrincipales => PuntosPrincipales?.Any() == true;
    }

    public class CreateDevocionalDto
    {
        [Required(ErrorMessage = "El título es requerido")]
        [MaxLength(200, ErrorMessage = "El título no puede exceder los 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        public DateTime? FechaProgramada { get; set; }

        public int? VoluntarioAsignadoId { get; set; }
        public string? VoluntarioAsignado { get; set; }

        [Required(ErrorMessage = "El pasaje bíblico es requerido")]
        [MaxLength(200, ErrorMessage = "El pasaje no puede exceder los 200 caracteres")]
        public string Pasaje { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? TextoClave { get; set; }

        [MaxLength(1000)]
        public string? Objetivo { get; set; }

        [MaxLength(1000)]
        public string? Idea { get; set; }

        public List<PuntoPrincipal>? PuntosPrincipales { get; set; }

        [MaxLength(2000)]
        public string? Aplicacion { get; set; }

        [MaxLength(1000)]
        public string? Reto { get; set; }

        [MaxLength(1000)]
        public string? Oracion { get; set; }

        [MaxLength(2000)]
        public string? Recursos { get; set; }

        public DevocionalEstado Estado { get; set; } = DevocionalEstado.Borrador;
    }

    public class UpdateDevocionalDto
    {
        [MaxLength(200)]
        public string? Titulo { get; set; }

        public DateTime? FechaProgramada { get; set; }
        
        public int? VoluntarioAsignadoId { get; set; }
        public string? VoluntarioAsignado { get; set; }

        [MaxLength(200)]
        public string? Pasaje { get; set; }

        [MaxLength(1000)]
        public string? TextoClave { get; set; }

        [MaxLength(1000)]
        public string? Objetivo { get; set; }

        [MaxLength(1000)]
        public string? Idea { get; set; }

        public List<PuntoPrincipal>? PuntosPrincipales { get; set; }

        [MaxLength(2000)]
        public string? Aplicacion { get; set; }

        [MaxLength(1000)]
        public string? Reto { get; set; }

        [MaxLength(1000)]
        public string? Oracion { get; set; }

        [MaxLength(2000)]
        public string? Recursos { get; set; }

        public DevocionalEstado? Estado { get; set; }
        public bool? IsActive { get; set; }
    }

    // DTOs específicos para el wizard
    public class DevocionalStep1Dto
    {
        [Required(ErrorMessage = "El título es requerido")]
        public string Titulo { get; set; } = string.Empty;
        
        public DateTime? FechaProgramada { get; set; }
        public string? VoluntarioAsignado { get; set; }
        public int? VoluntarioAsignadoId { get; set; }
    }

    public class DevocionalStep2Dto
    {
        [Required(ErrorMessage = "El pasaje es requerido")]
        public string Pasaje { get; set; } = string.Empty;
        
        public string? TextoClave { get; set; }
    }

    public class DevocionalStep3Dto
    {
        // Mensaje Principal
        public string? Objetivo { get; set; }
        public string? Idea { get; set; }
        public List<PuntoPrincipal>? PuntosPrincipales { get; set; }

        // Aplicación Práctica  
        public string? Aplicacion { get; set; }
        public string? Reto { get; set; }
        public string? Oracion { get; set; }

        // Recursos Extra
        public string? Recursos { get; set; }
    }

    public class SaveDraftDto
    {
        public int? DevocionalId { get; set; }
        public string? Titulo { get; set; }
        public DateTime? FechaProgramada { get; set; }
        public string? VoluntarioAsignado { get; set; }
        public int? VoluntarioAsignadoId { get; set; }
        public string? Pasaje { get; set; }
        public string? TextoClave { get; set; }
        public string? Objetivo { get; set; }
        public string? Idea { get; set; }
        public List<PuntoPrincipal>? PuntosPrincipales { get; set; }
        public string? Aplicacion { get; set; }
        public string? Reto { get; set; }
        public string? Oracion { get; set; }
        public string? Recursos { get; set; }
    }

    public class DevocionalPreviewDto
    {
        public string Titulo { get; set; } = string.Empty;
        public DateTime? FechaProgramada { get; set; }
        public string? VoluntarioAsignado { get; set; }
        public string? Pasaje { get; set; }
        public string? Objetivo { get; set; }
        public string? Idea { get; set; }
        public string? ContentSummary { get; set; }
        public bool HasContent { get; set; }
    }

    public class DevocionalEstadisticasDto
    {
        public int TotalDevocionales { get; set; }
        public int BorradoresCount { get; set; }
        public int ProgramadosCount { get; set; }
        public int CompletadosCount { get; set; }
        public int CanceladosCount { get; set; }
        public int DevocionalesTodayCount { get; set; }
        public int DevocionalesTomorrow { get; set; }
        public int DevocionalessemanaCount { get; set; }
        public List<DevocionalPorMesDto>? DevocionalPorMes { get; set; }
        public List<VoluntarioDevocionalDto>? TopVoluntarios { get; set; }
    }

    public class DevocionalPorMesDto
    {
        public int Mes { get; set; }
        public string NombreMes { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class VoluntarioDevocionalDto
    {
        public int VoluntarioId { get; set; }
        public string NombreVoluntario { get; set; } = string.Empty;
        public int TotalDevocionales { get; set; }
        public int Devocionalcompletados { get; set; }
    }
}