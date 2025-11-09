using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiPG.DTOs
{
    public class CrearAsistenciaDto
    {
        [Required(ErrorMessage = "El NivelId es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El NivelId debe ser mayor a 0")]
        [JsonPropertyName("levelId")]
        public int NivelId { get; set; }

        [Required(ErrorMessage = "El UsuarioId es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El UsuarioId debe ser mayor a 0")]
        [JsonPropertyName("userId")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [JsonPropertyName("date")]
        public DateTime Fecha { get; set; }

        [JsonPropertyName("present")]
        public bool Presente { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        [JsonPropertyName("remarks")]
        public string? Observaciones { get; set; }
    }

    public class ReporteAsistenciaDto
    {
        public int Id { get; set; }
        public int NivelId { get; set; }
        public int UsuarioId { get; set; }
        public string? NombreUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public bool Presente { get; set; }
        public string? Observaciones { get; set; }
        public DateTime CreadoEn { get; set; }
    }
}
