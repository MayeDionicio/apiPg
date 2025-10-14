namespace ApiPG.Models
{
    public class NivelDeParticipante
    {
        public int IdNivel { get; set; }
        public Nivel Nivel { get; set; } = null!;

        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public DateTime AsignadoEn { get; set; } = DateTime.UtcNow;
        public bool EstaActivo { get; set; } = true; // Para soft-assignment
    }
}
