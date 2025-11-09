using System.ComponentModel.DataAnnotations;

namespace ApiPG.Models
{
    public class TokenRecuperacion
    {
        public int Id { get; set; }

        [Required]
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime FechaExpiracion { get; set; }

        public bool Usado { get; set; } = false;

        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}
