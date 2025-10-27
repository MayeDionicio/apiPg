namespace ApiPG.DTOs
{
    public class CrearActividadDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string AreaDeEnfoque { get; set; } = string.Empty;
        public string? DescripcionDetallada { get; set; }
        public int DuracionEstimadaMinutos { get; set; }
        public string? MaterialesNecesarios { get; set; }
        public DateTime FechaDelEvento { get; set; }
        public List<int> VoluntariosIds { get; set; } = new List<int>();
    }

    public class ActualizarActividadDto
    {
        public string? Titulo { get; set; }
        public string? AreaDeEnfoque { get; set; }
        public string? DescripcionDetallada { get; set; }
        public int? DuracionEstimadaMinutos { get; set; }
        public string? MaterialesNecesarios { get; set; }
        public DateTime? FechaDelEvento { get; set; }
        public List<int>? VoluntariosIds { get; set; }
    }

    public class ActividadDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string AreaDeEnfoque { get; set; } = string.Empty;
        public string? DescripcionDetallada { get; set; }
        public int DuracionEstimadaMinutos { get; set; }
        public string? MaterialesNecesarios { get; set; }
        public DateTime FechaDelEvento { get; set; }
        public int CreadoPorIdUsuario { get; set; }
        public string NombreCoordinador { get; set; } = string.Empty;
        public List<VoluntarioAsignadoDto> Voluntarios { get; set; } = new List<VoluntarioAsignadoDto>();
        public DateTime CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public bool EstaActivo { get; set; }
    }

    public class VoluntarioAsignadoDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NombreVoluntario { get; set; } = string.Empty;
        public string EmailVoluntario { get; set; } = string.Empty;
        public DateTime AsignadoEn { get; set; }
    }

    public class EstadisticasActividadesDto
    {
        public int TotalActividades { get; set; }
        public int ActividadesPendientes { get; set; }
        public int ActividadesRealizadas { get; set; }
        public int ActividadesEsteMes { get; set; }
        public double PromedioVoluntariosPorActividad { get; set; }
        public double PromedioDuracionMinutos { get; set; }
    }
}
