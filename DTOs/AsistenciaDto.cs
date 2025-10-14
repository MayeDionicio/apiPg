namespace ApiPG.DTOs
{
    public class CrearAsistenciaDto
    {
        public int NivelId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public bool Presente { get; set; }
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
