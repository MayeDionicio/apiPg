using System.ComponentModel.DataAnnotations;

namespace ApiPG.Models
{
    public class Nivel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        // Tutor del nivel: un voluntario (User)
        public int? IdVoluntario { get; set; }
        public Usuario? Voluntario { get; set; }

        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public bool EstaActivo { get; set; } = true;

        // Participantes (many-to-many a través de NivelDeParticipante)
        public ICollection<NivelDeParticipante> ParticipantesDelNivel { get; set; } = new List<NivelDeParticipante>();
    }
}
