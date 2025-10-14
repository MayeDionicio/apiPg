using System.ComponentModel.DataAnnotations;
using ApiPG.Models;

namespace ApiPG.DTOs
{
    // DTOs para Resource
    public class CrearRecursoDto
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        [StringLength(50)]
        public string Categoria { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [StringLength(50)]
        public string? Unidad { get; set; }

        [StringLength(200)]
        public string? Ubicacion { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El valor estimado debe ser positivo")]
        public decimal? ValorEstimado { get; set; }

        [StringLength(1000)]
        public string? Notas { get; set; }
    }

    public class ActualizarRecursoDto
    {
        [StringLength(100)]
        public string? Nombre { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [StringLength(50)]
        public string? Categoria { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int? Cantidad { get; set; }

        [StringLength(50)]
        public string? Unidad { get; set; }

        [StringLength(200)]
        public string? Ubicacion { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El valor estimado debe ser positivo")]
        public decimal? ValorEstimado { get; set; }

        [StringLength(1000)]
        public string? Notas { get; set; }

        public bool? EstaActivo { get; set; }
    }

    public class RecursoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public int CantidadDisponible { get; set; }
        public string? Unidad { get; set; }
        public string? Ubicacion { get; set; }
        public decimal? ValorEstimado { get; set; }
        public string? Notas { get; set; }
        public DateTime CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public bool EstaActivo { get; set; }
        public int CantidadAsignaciones { get; set; } // Número total de asignaciones
        public int CantidadAsignacionesActivas { get; set; } // Asignaciones activas
    }

    // DTOs para ResourceAssignment
    public class CrearAsignacionDeRecursoDto
    {
        [Required]
        public int RecursoId { get; set; }

        [Required]
        public int VoluntarioId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int CantidadAsignada { get; set; }

        public DateTime? FechaEsperadaDeDevolucion { get; set; }

        [StringLength(1000)]
        public string? NotasIniciales { get; set; }
    }

    public class ActualizarAsignacionDeRecursoDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int? CantidadAsignada { get; set; }

        public EstadoDeAsignacion? Estado { get; set; }

        public DateTime? FechaEsperadaDeDevolucion { get; set; }

        [StringLength(1000)]
        public string? NotasIniciales { get; set; }

        [StringLength(1000)]
        public string? NotasDelVoluntario { get; set; }
    }

    public class ConfirmarAsignacionDto
    {
        [StringLength(1000)]
        public string? NotasDelVoluntario { get; set; }
    }

    public class AsignacionDeRecursoDto
    {
        public int Id { get; set; }
        public int RecursoId { get; set; }
        public string NombreRecurso { get; set; } = string.Empty;
        public string CategoriaRecurso { get; set; } = string.Empty;
        public int VoluntarioId { get; set; }
        public string NombreVoluntario { get; set; } = string.Empty;
        public int CantidadAsignada { get; set; }
        public EstadoDeAsignacion Estado { get; set; }
        public DateTime AsignadoEn { get; set; }
        public DateTime? ConfirmadoEn { get; set; }
        public DateTime? IniciadoEn { get; set; }
        public DateTime? DevueltoEn { get; set; }
        public DateTime? FechaEsperadaDeDevolucion { get; set; }
        public string? NotasIniciales { get; set; }
        public string? NotasDelVoluntario { get; set; }
        public int? AsignadoPorIdUsuario { get; set; }
        public string? AsignadoPorNombreUsuario { get; set; }
        public bool EstaActivo { get; set; }
        public int CantidadRegistrosDeUso { get; set; }
        public bool TieneIncidentes { get; set; }
    }

    // DTOs para ResourceUsageLog
    public class CrearRegistroDeUsoDeRecursoDto
    {
        [Required]
        public int AsignacionDeRecursoId { get; set; }

        [Required]
        public TipoDeEventoDeUso TipoDeEvento { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Descripcion { get; set; } = string.Empty;

        public CondicionDelRecurso? CondicionAntes { get; set; }

        public CondicionDelRecurso? CondicionDespues { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad afectada debe ser mayor a 0")]
        public int? CantidadAfectada { get; set; }

        [StringLength(1000)]
        public string? AccionesTomadas { get; set; }

        [StringLength(1000)]
        public string? Recomendaciones { get; set; }

        public bool RequiereSeguimiento { get; set; } = false;

        [StringLength(500)]
        public string? UrlsDeFotos { get; set; }
    }

    public class RegistroDeUsoDeRecursoDto
    {
        public int Id { get; set; }
        public int AsignacionDeRecursoId { get; set; }
        public string NombreRecurso { get; set; } = string.Empty;
        public string NombreVoluntario { get; set; } = string.Empty;
        public TipoDeEventoDeUso TipoDeEvento { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public CondicionDelRecurso? CondicionAntes { get; set; }
        public CondicionDelRecurso? CondicionDespues { get; set; }
        public int? CantidadAfectada { get; set; }
        public string? AccionesTomadas { get; set; }
        public string? Recomendaciones { get; set; }
        public bool RequiereSeguimiento { get; set; }
        public DateTime CreadoEn { get; set; }
        public int ReportadoPorIdUsuario { get; set; }
        public string ReportadoPorNombreUsuario { get; set; } = string.Empty;
        public string? UrlsDeFotos { get; set; }
        public bool EstaResuelto { get; set; }
        public DateTime? ResueltoEn { get; set; }
        public int? ResueltoPorIdUsuario { get; set; }
        public string? ResueltoPorNombreUsuario { get; set; }
        public string? NotasDeResolucion { get; set; }
    }

    public class ResolverIncidenteDto
    {
        [Required]
        [StringLength(1000)]
        public string NotasDeResolucion { get; set; } = string.Empty;
    }
}