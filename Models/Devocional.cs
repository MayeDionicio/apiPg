using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPG.Models
{
    public class Devocional
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime? FechaProgramada { get; set; }

        public int? VoluntarioAsignadoId { get; set; }

        [MaxLength(100)]
        public string? VoluntarioAsignado { get; set; } // Nombre del voluntario

        [MaxLength(200)]
        public string? Pasaje { get; set; }

        [MaxLength(1000)]
        public string? TextoClave { get; set; }

        [MaxLength(1000)]
        public string? Objetivo { get; set; }

        [MaxLength(1000)]
        public string? Idea { get; set; }

        // Puntos principales almacenados como JSON
        public string? PuntosPrincipales { get; set; }

        [MaxLength(2000)]
        public string? Aplicacion { get; set; }

        [MaxLength(1000)]
        public string? Reto { get; set; }

        [MaxLength(1000)]
        public string? Oracion { get; set; }

        [MaxLength(2000)]
        public string? Recursos { get; set; }

        public DevocionalEstado Estado { get; set; } = DevocionalEstado.Borrador;

        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime? ActualizadoEn { get; set; }
        
        public int CreadoPorIdUsuario { get; set; }
        public int? ActualizadoPorIdUsuario { get; set; }

        public bool EstaActivo { get; set; } = true;

        // Navegación
        public virtual Usuario CreadoPorUsuario { get; set; } = null!;
        public virtual Usuario? ActualizadoPorUsuario { get; set; }
        public virtual Usuario? VoluntarioAsignadoUser { get; set; }
    }

    public enum DevocionalEstado
    {
        Borrador = 0,
        Programado = 1,
        EnProgreso = 2,
        Completado = 3,
        Cancelado = 4
    }

    // Clase para los puntos principales
    public class PuntoPrincipal
    {
        public string T { get; set; } = string.Empty; // Título
        public string? V { get; set; } // Versículo
        public string? D { get; set; } // Desarrollo/Descripción
    }
}