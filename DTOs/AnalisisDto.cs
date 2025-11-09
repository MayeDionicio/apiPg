namespace ApiPG.DTOs
{
    /// <summary>
    /// DTOs para métricas y análisis del dashboard
    /// </summary>
    
    public class MetricasGeneralesDto
    {
        public CrecimientoUsuariosDto CrecimientoUsuarios { get; set; } = new();
        public ActividadesCompletadasDto ActividadesCompletadas { get; set; } = new();
        public TiempoDeUsoDto TiempoDeUso { get; set; } = new();
    }

    public class CrecimientoUsuariosDto
    {
        public int NuevosUsuariosEsteMes { get; set; }
        public decimal TasaRetencion { get; set; } // Porcentaje
        public int UsuariosTotales { get; set; }
        public int UsuariosActivosEsteMes { get; set; }
        public List<UsuariosPorMesDto> CrecimientoPorMes { get; set; } = new();
    }

    public class UsuariosPorMesDto
    {
        public int Mes { get; set; }
        public int Anio { get; set; }
        public string NombreMes { get; set; } = string.Empty;
        public int CantidadNuevos { get; set; }
        public int CantidadTotal { get; set; }
    }

    public class ActividadesCompletadasDto
    {
        public int ActividadesTotales { get; set; }
        public int ActividadesCompletadas { get; set; }
        public decimal PorcentajeCompletacion { get; set; }
        public int TareasCompletadas { get; set; }
        public int TareasTotales { get; set; }
        public List<ActividadesPorMesDto> CompletacionPorMes { get; set; } = new();
    }

    public class ActividadesPorMesDto
    {
        public int Mes { get; set; }
        public int Anio { get; set; }
        public string NombreMes { get; set; } = string.Empty;
        public int Completadas { get; set; }
        public int Totales { get; set; }
        public decimal Porcentaje { get; set; }
    }

    public class TiempoDeUsoDto
    {
        public decimal PromedioHorasDiarias { get; set; }
        public decimal PorcentajeUsuariosActivos { get; set; }
        public int UsuariosActivosHoy { get; set; }
        public int UsuariosTotales { get; set; }
        public int SesionesTotales { get; set; }
        public decimal TiempoTotalHoras { get; set; }
    }

    public class MetricasPorRolDto
    {
        public string NombreRol { get; set; } = string.Empty;
        public int CantidadUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public decimal PorcentajeActivos { get; set; }
    }

    public class EstadisticasDetalladasDto
    {
        // Usuarios
        public int TotalUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public int UsuariosInactivos { get; set; }
        public List<MetricasPorRolDto> UsuariosPorRol { get; set; } = new();

        // Niveles
        public int TotalNiveles { get; set; }
        public int NivelesActivos { get; set; }
        public decimal PromedioParticipantesPorNivel { get; set; }

        // Actividades
        public int TotalActividades { get; set; }
        public int ActividadesActivas { get; set; }
        public int ActividadesMontessori { get; set; }

        // Tareas
        public int TotalTareas { get; set; }
        public int TareasCompletadas { get; set; }
        public int TareasPendientes { get; set; }
        public int TareasEnProceso { get; set; }

        // Asistencia
        public int RegistrosAsistenciaHoy { get; set; }
        public int RegistrosAsistenciaEsteMes { get; set; }
        public decimal PorcentajeAsistencia { get; set; }
    }

    public class RangoFechasDto
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
