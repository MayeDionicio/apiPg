namespace ApiPG.DTOs
{
    public class ParticipanteDeNivelDto
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime AsignadoEn { get; set; }
        public bool EstaActivo { get; set; }
    }
}
