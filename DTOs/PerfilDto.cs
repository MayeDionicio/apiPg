using System.ComponentModel.DataAnnotations;

namespace ApiPG.DTOs
{
    public class CrearPerfilDto
    {
        public string? FotoPerfil { get; set; }
        
        [Phone]
        public string? Telefono { get; set; }
        
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Pais { get; set; }
        
        // Contacto de emergencia
        public string? ContactoEmergenciaNombre { get; set; }
        public string? ContactoEmergenciaTelefono { get; set; }
        public string? ContactoEmergenciaRelacion { get; set; }
        
        // Información familiar (para participantes)
        public string? NombrePadre { get; set; }
        public string? NombreMadre { get; set; }
        public string? NombreTutor { get; set; }
        
        // Información médica
        public string? Alergias { get; set; }
        public string? CondicionesMedicas { get; set; }
        public string? Medicamentos { get; set; }
        
        // Información personal
        public string? Biografia { get; set; }
        public string? Intereses { get; set; }
        
        // Redes sociales
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Twitter { get; set; }
    }

    public class ActualizarPerfilDto
    {
        public string? FotoPerfil { get; set; }
        
        [Phone]
        public string? Telefono { get; set; }
        
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Pais { get; set; }
        
        public string? ContactoEmergenciaNombre { get; set; }
        public string? ContactoEmergenciaTelefono { get; set; }
        public string? ContactoEmergenciaRelacion { get; set; }
        
        public string? NombrePadre { get; set; }
        public string? NombreMadre { get; set; }
        public string? NombreTutor { get; set; }
        
        public string? Alergias { get; set; }
        public string? CondicionesMedicas { get; set; }
        public string? Medicamentos { get; set; }
        
        public string? Biografia { get; set; }
        public string? Intereses { get; set; }
        
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Twitter { get; set; }
    }

    public class PerfilUsuarioDto
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        
        // Información del usuario
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreRol { get; set; } = string.Empty;
        public DateTime? FechaDeNacimiento { get; set; }
        public int? Edad { get; set; }
        
        // Información del perfil
        public string? FotoPerfil { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Pais { get; set; }
        
        public string? ContactoEmergenciaNombre { get; set; }
        public string? ContactoEmergenciaTelefono { get; set; }
        public string? ContactoEmergenciaRelacion { get; set; }
        
        public string? NombrePadre { get; set; }
        public string? NombreMadre { get; set; }
        public string? NombreTutor { get; set; }
        
        public string? Alergias { get; set; }
        public string? CondicionesMedicas { get; set; }
        public string? Medicamentos { get; set; }
        
        public string? Biografia { get; set; }
        public string? Intereses { get; set; }
        
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Twitter { get; set; }
        
        public DateTime CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
    }

    public class SubirFotoPerfilDto
    {
        [Required]
        public string FotoBase64 { get; set; } = string.Empty;
        
        public string? NombreArchivo { get; set; }
    }
}
