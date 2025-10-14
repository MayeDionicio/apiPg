using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPG.Models
{
    public enum EstadoDeAsignacion
    {
        Pendiente,        // Asignado pero no confirmado por el voluntario
        Confirmado,      // Confirmado por el voluntario
        EnUso,          // En uso actualmente
        Devuelto,       // Devuelto
        Cancelado       // Cancelado
    }

    public class AsignacionDeRecurso
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdRecurso { get; set; }

        [Required]
        public int IdVoluntario { get; set; } // UserId del voluntario

        [Required]
        public int CantidadAsignada { get; set; }

        [Required]
        public EstadoDeAsignacion Estado { get; set; } = EstadoDeAsignacion.Pendiente;

        public DateTime AsignadoEn { get; set; } = DateTime.UtcNow;

        public DateTime? ConfirmadoEn { get; set; } // Cuando el voluntario confirma

        public DateTime? IniciadoEn { get; set; } // Cuando inicia el uso

        public DateTime? DevueltoEn { get; set; } // Cuando se devuelve

        public DateTime? FechaEsperadaDeDevolucion { get; set; } // Fecha esperada de devolución

        [StringLength(1000)]
        public string? NotasIniciales { get; set; } // Notas al momento de la asignación

        [StringLength(1000)]
        public string? NotasDelVoluntario { get; set; } // Notas del voluntario al confirmar

        public int? AsignadoPorIdUsuario { get; set; } // Usuario que asignó el recurso

        [Required]
        public bool EstaActivo { get; set; } = true;

        // Navigation properties
        [ForeignKey("IdRecurso")]
        public Recurso Recurso { get; set; } = null!;

        [ForeignKey("IdVoluntario")]
        public Usuario Voluntario { get; set; } = null!;

        [ForeignKey("AsignadoPorIdUsuario")]
        public Usuario? AsignadoPor { get; set; }

        public ICollection<RegistroDeUsoDeRecurso> RegistrosDeUso { get; set; } = new List<RegistroDeUsoDeRecurso>();
    }
}