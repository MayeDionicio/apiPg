using System.ComponentModel.DataAnnotations;

namespace ApiPG.Models
{
    public enum EstadoTarea
    {
        Pendiente = 0,      // Asignada pero no vista
        Vista = 1,          // El voluntario la vio
        EnProceso = 2,      // El voluntario está trabajando en ella
        Completada = 3,     // El voluntario la marcó como terminada
        Aprobada = 4,       // El coordinador la aprobó
        Rechazada = 5       // El coordinador la rechazó
    }

    public class Tarea
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Descripcion { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        // Coordinador que crea/asigna la tarea
        public int CreadoPorIdUsuario { get; set; }
        public Usuario? CreadoPor { get; set; }

        // Voluntarios asignados (relación many-to-many)
        public ICollection<AsignacionDeTarea> AsignacionesDeTarea { get; set; } = new List<AsignacionDeTarea>();

        public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente;

        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        public DateTime? ActualizadoEn { get; set; }

        public bool EstaActivo { get; set; } = true;

        // Notas adicionales del coordinador
        [MaxLength(1000)]
        public string? NotasCoordinador { get; set; }

        // Notas del voluntario al completar
        [MaxLength(1000)]
        public string? NotasVoluntario { get; set; }

        // Fecha cuando fue aprobada/rechazada
        public DateTime? FechaRevision { get; set; }

        // Razón de rechazo si aplica
        [MaxLength(500)]
        public string? RazonRechazo { get; set; }
    }

    // Tabla intermedia para many-to-many entre Tarea y Usuario (voluntarios)
    public class AsignacionDeTarea
    {
        public int Id { get; set; }

        public int TareaId { get; set; }
        public Tarea? Tarea { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        // Estado específico para este voluntario
        public EstadoTarea EstadoVoluntario { get; set; } = EstadoTarea.Pendiente;

        public DateTime AsignadoEn { get; set; } = DateTime.UtcNow;

        // Cuándo vio la tarea por primera vez
        public DateTime? VistaEn { get; set; }

        // Cuándo comenzó a trabajar en ella
        public DateTime? IniciadaEn { get; set; }

        // Cuándo la completó
        public DateTime? CompletadaEn { get; set; }

        public bool EstaActivo { get; set; } = true;
    }
}
