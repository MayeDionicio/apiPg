namespace ApiPG.Models
{
    public class Actividad
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string AreaDeEnfoque { get; set; } = string.Empty;
        public string? DescripcionDetallada { get; set; }
        public int DuracionEstimadaMinutos { get; set; }
        public string? MaterialesNecesarios { get; set; }
        public DateTime FechaDelEvento { get; set; }
        public int CreadoPorIdUsuario { get; set; }
        public Usuario? CreadoPor { get; set; }
        
        // Auditoría
        public DateTime CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public bool EstaActivo { get; set; }

        // Relación con voluntarios asignados
        public ICollection<AsignacionDeActividad> AsignacionesDeActividad { get; set; } = new List<AsignacionDeActividad>();
    }

    public class AsignacionDeActividad
    {
        public int Id { get; set; }
        public int ActividadId { get; set; }
        public Actividad? Actividad { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public DateTime AsignadoEn { get; set; }
        public bool EstaActivo { get; set; }
    }
}
