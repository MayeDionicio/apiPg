using System.ComponentModel.DataAnnotations;

namespace ApiPG.Models
{
    public class Asistencia
    {
        public int Id { get; set; }

        public int IdNivel { get; set; }
        public Nivel Nivel { get; set; } = null!;

        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

        // Date for which attendance is recorded (use UTC date component)
        public DateTime Fecha { get; set; }

        // true = present, false = absent
        public bool Presente { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime? ActualizadoEn { get; set; }

        // We keep records immutable for deletion purposes; soft-delete flag kept but we'll not expose delete endpoint
        public bool EstaEliminado { get; set; } = false;
    }
}
