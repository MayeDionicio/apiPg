using System.ComponentModel.DataAnnotations;

namespace ApiPG.DTOs
{
    // DTO para crear un logro (solo coordinadores)
    public class CrearLogroFacilitadorDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(1000)]
        public string Descripcion { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Categoria { get; set; }
        
        [StringLength(50)]
        public string? Icono { get; set; }
        
        [Required(ErrorMessage = "Los puntos son requeridos")]
        [Range(1, 1000, ErrorMessage = "Los puntos deben estar entre 1 y 1000")]
        public int PuntosValor { get; set; }
        
        [StringLength(500)]
        public string? CriteriosObtencion { get; set; }
        
        public string TipoLogro { get; set; } = "Manual";
        
        public int? CantidadActividadesRequeridas { get; set; }
        public int? CantidadEstudiantesRequeridos { get; set; }
        public int? DiasConsecutivosRequeridos { get; set; }
    }

    // DTO para actualizar un logro
    public class ActualizarLogroFacilitadorDto
    {
        [StringLength(200)]
        public string? Nombre { get; set; }
        
        [StringLength(1000)]
        public string? Descripcion { get; set; }
        
        [StringLength(100)]
        public string? Categoria { get; set; }
        
        [StringLength(50)]
        public string? Icono { get; set; }
        
        [Range(1, 1000)]
        public int? PuntosValor { get; set; }
        
        [StringLength(500)]
        public string? CriteriosObtencion { get; set; }
        
        public int? CantidadActividadesRequeridas { get; set; }
        public int? CantidadEstudiantesRequeridos { get; set; }
        public int? DiasConsecutivosRequeridos { get; set; }
    }

    // DTO para otorgar un logro a un facilitador
    public class OtorgarLogroDto
    {
        [Required(ErrorMessage = "El ID del logro es requerido")]
        public int IdLogro { get; set; }
        
        [Required(ErrorMessage = "El ID del facilitador es requerido")]
        public int IdFacilitador { get; set; }
        
        public int? IdActividadRelacionada { get; set; }
        
        public int? IdActividadMontessoriRelacionada { get; set; }
        
        public int? IdActividadPersonalizadaRelacionada { get; set; }
        
        [StringLength(1000)]
        public string? Justificacion { get; set; }
        
        [StringLength(500)]
        public string? ComentarioCoordinador { get; set; }
    }

    // DTO de respuesta para logro
    public class LogroFacilitadorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public string? Icono { get; set; }
        public int PuntosValor { get; set; }
        public string? CriteriosObtencion { get; set; }
        public string TipoLogro { get; set; } = string.Empty;
        public int? CantidadActividadesRequeridas { get; set; }
        public int? CantidadEstudiantesRequeridos { get; set; }
        public int? DiasConsecutivosRequeridos { get; set; }
        public int CreadoPorIdUsuario { get; set; }
        public string NombreCreadoPor { get; set; } = string.Empty;
        public DateTime CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public bool EstaActivo { get; set; }
        public int TotalVecesOtorgado { get; set; } // Cuántos facilitadores lo tienen
    }

    // DTO de respuesta para logro obtenido
    public class LogroObtenidoFacilitadorDto
    {
        public int Id { get; set; }
        public int IdLogro { get; set; }
        public string NombreLogro { get; set; } = string.Empty;
        public string DescripcionLogro { get; set; } = string.Empty;
        public string? CategoriaLogro { get; set; }
        public string? IconoLogro { get; set; }
        public int PuntosLogro { get; set; }
        public int IdFacilitador { get; set; }
        public string NombreFacilitador { get; set; } = string.Empty;
        public DateTime FechaObtencion { get; set; }
        public int OtorgadoPorIdUsuario { get; set; }
        public string NombreOtorgadoPor { get; set; } = string.Empty;
        public int? IdActividadRelacionada { get; set; }
        public string? NombreActividadRelacionada { get; set; }
        public int? IdActividadMontessoriRelacionada { get; set; }
        public string? NombreActividadMontessoriRelacionada { get; set; }
        public int? IdActividadPersonalizadaRelacionada { get; set; }
        public string? NombreActividadPersonalizadaRelacionada { get; set; }
        public string? Justificacion { get; set; }
        public string? ComentarioCoordinador { get; set; }
        public bool EsVisible { get; set; }
        public DateTime CreadoEn { get; set; }
    }

    // DTO para el perfil de logros de un facilitador
    public class PerfilLogrosFacilitadorDto
    {
        public int IdFacilitador { get; set; }
        public string NombreFacilitador { get; set; } = string.Empty;
        public string EmailFacilitador { get; set; } = string.Empty;
        public int TotalLogrosObtenidos { get; set; }
        public int TotalPuntosAcumulados { get; set; }
        public List<LogroObtenidoFacilitadorDto> LogrosObtenidos { get; set; } = new();
        public Dictionary<string, int>? LogrosPorCategoria { get; set; }
        public DateTime? PrimerLogroObtenido { get; set; }
        public DateTime? UltimoLogroObtenido { get; set; }
        public int RankingPosicion { get; set; } // Posición en el ranking general
        public int TotalFacilitadores { get; set; } // Total de facilitadores para contexto
    }

    // DTO para ranking de facilitadores
    public class RankingFacilitadorDto
    {
        public int Posicion { get; set; }
        public int IdFacilitador { get; set; }
        public string NombreFacilitador { get; set; } = string.Empty;
        public string EmailFacilitador { get; set; } = string.Empty;
        public int TotalLogros { get; set; }
        public int TotalPuntos { get; set; }
        public DateTime? UltimoLogroFecha { get; set; }
    }

    // DTO para filtros de logros
    public class FiltrosLogrosDto
    {
        public string? Categoria { get; set; }
        public string? TipoLogro { get; set; }
        public bool? SoloActivos { get; set; } = true;
        public int? PuntosMinimos { get; set; }
        public int? PuntosMaximos { get; set; }
    }

    // DTO para filtros de logros obtenidos
    public class FiltrosLogrosObtenidosDto
    {
        public int? IdFacilitador { get; set; }
        public int? IdLogro { get; set; }
        public string? Categoria { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? OtorgadoPorIdUsuario { get; set; }
    }

    // DTO para estadísticas generales
    public class EstadisticasLogrosDto
    {
        public int TotalLogrosDefinidos { get; set; }
        public int TotalLogrosActivos { get; set; }
        public int TotalLogrosOtorgados { get; set; }
        public int TotalFacilitadoresConLogros { get; set; }
        public int TotalPuntosOtorgados { get; set; }
        public Dictionary<string, int>? LogrosOtorgadosPorCategoria { get; set; }
        public List<LogroFacilitadorDto>? LogrosMasOtorgados { get; set; }
        public List<RankingFacilitadorDto>? TopFacilitadores { get; set; }
    }
}
