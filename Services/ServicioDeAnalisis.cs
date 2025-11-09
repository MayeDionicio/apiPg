using ApiPG.Data;
using ApiPG.DTOs;
using ApiPG.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ApiPG.Services
{
    public interface IServicioDeAnalisis
    {
        Task<MetricasGeneralesDto> ObtenerMetricasGeneralesAsync();
        Task<CrecimientoUsuariosDto> ObtenerCrecimientoUsuariosAsync(int meses = 6);
        Task<ActividadesCompletadasDto> ObtenerActividadesCompletadasAsync();
        Task<TiempoDeUsoDto> ObtenerTiempoDeUsoAsync();
        Task<EstadisticasDetalladasDto> ObtenerEstadisticasDetalladasAsync();
    }

    public class ServicioDeAnalisis : IServicioDeAnalisis
    {
        private readonly ApiPGContext _db;

        public ServicioDeAnalisis(ApiPGContext db)
        {
            _db = db;
        }

        public async Task<MetricasGeneralesDto> ObtenerMetricasGeneralesAsync()
        {
            var metricas = new MetricasGeneralesDto
            {
                CrecimientoUsuarios = await ObtenerCrecimientoUsuariosAsync(),
                ActividadesCompletadas = await ObtenerActividadesCompletadasAsync(),
                TiempoDeUso = await ObtenerTiempoDeUsoAsync()
            };

            return metricas;
        }

        public async Task<CrecimientoUsuariosDto> ObtenerCrecimientoUsuariosAsync(int meses = 6)
        {
            var hoy = DateTime.UtcNow;
            var inicioMesActual = DateTime.SpecifyKind(new DateTime(hoy.Year, hoy.Month, 1), DateTimeKind.Utc);
            var fechaInicio = inicioMesActual.AddMonths(-meses);

            // Usuarios nuevos este mes
            var nuevosEsteMes = await _db.Usuarios
                .CountAsync(u => u.CreadoEn >= inicioMesActual);

            // Total de usuarios
            var usuariosTotales = await _db.Usuarios.CountAsync();

            // Usuarios activos este mes (que tienen actividad reciente)
            var usuariosActivosEsteMes = await _db.Usuarios
                .CountAsync(u => u.EstaActivo && u.ActualizadoEn >= inicioMesActual);

            // Si no hay ActualizadoEn, usar usuarios activos
            if (usuariosActivosEsteMes == 0)
            {
                usuariosActivosEsteMes = await _db.Usuarios.CountAsync(u => u.EstaActivo);
            }

            // Calcular tasa de retención (usuarios activos / usuarios totales)
            var tasaRetencion = usuariosTotales > 0 
                ? Math.Round((decimal)usuariosActivosEsteMes / usuariosTotales * 100, 1)
                : 0;

            // Crecimiento por mes
            var usuarios = await _db.Usuarios
                .Where(u => u.CreadoEn >= fechaInicio)
                .OrderBy(u => u.CreadoEn)
                .Select(u => new { u.CreadoEn })
                .ToListAsync();

            var crecimientoPorMes = usuarios
                .GroupBy(u => new { u.CreadoEn.Year, u.CreadoEn.Month })
                .Select(g => new UsuariosPorMesDto
                {
                    Anio = g.Key.Year,
                    Mes = g.Key.Month,
                    NombreMes = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy", new CultureInfo("es-ES")),
                    CantidadNuevos = g.Count(),
                    CantidadTotal = usuarios.Count(u => u.CreadoEn <= DateTime.SpecifyKind(new DateTime(g.Key.Year, g.Key.Month, DateTime.DaysInMonth(g.Key.Year, g.Key.Month), 23, 59, 59), DateTimeKind.Utc))
                })
                .OrderBy(x => x.Anio)
                .ThenBy(x => x.Mes)
                .ToList();

            return new CrecimientoUsuariosDto
            {
                NuevosUsuariosEsteMes = nuevosEsteMes,
                TasaRetencion = tasaRetencion,
                UsuariosTotales = usuariosTotales,
                UsuariosActivosEsteMes = usuariosActivosEsteMes,
                CrecimientoPorMes = crecimientoPorMes
            };
        }

        public async Task<ActividadesCompletadasDto> ObtenerActividadesCompletadasAsync()
        {
            // Actividades regulares
            var actividadesTotales = await _db.Actividades
                .CountAsync(a => a.EstaActivo);

            // Tareas
            var tareasTotales = await _db.Tareas
                .CountAsync(t => t.EstaActivo);

            var tareasCompletadas = await _db.Tareas
                .CountAsync(t => t.Estado == EstadoTarea.Completada || t.Estado == EstadoTarea.Aprobada);

            // Actividades Montessori
            var actividadesMontessoriTotales = await _db.ActividadesMontessori
                .CountAsync(a => a.EstaActivo);

            var logrosObtenidos = await _db.LogrosMontessori
                .CountAsync(l => l.EsObtenido);

            // Calcular totales combinados
            var actividadesGeneralesTotales = actividadesTotales + actividadesMontessoriTotales;
            var completacionTotal = tareasCompletadas + logrosObtenidos;
            var tareasGeneralesTotales = tareasTotales + actividadesMontessoriTotales;

            var porcentajeCompletacion = tareasGeneralesTotales > 0
                ? Math.Round((decimal)completacionTotal / tareasGeneralesTotales * 100, 1)
                : 0;

            // Completación por mes (últimos 6 meses)
            var hoy = DateTime.UtcNow;
            var fechaInicio = hoy.AddMonths(-6);

            var tareasCompletadasPorMes = await _db.Tareas
                .Where(t => t.FechaFin.HasValue && t.FechaFin >= fechaInicio && (t.Estado == EstadoTarea.Completada || t.Estado == EstadoTarea.Aprobada))
                .Select(t => new { t.FechaFin })
                .ToListAsync();

            var completacionPorMes = tareasCompletadasPorMes
                .Where(t => t.FechaFin.HasValue)
                .GroupBy(t => new { t.FechaFin!.Value.Year, t.FechaFin!.Value.Month })
                .Select(g => new ActividadesPorMesDto
                {
                    Anio = g.Key.Year,
                    Mes = g.Key.Month,
                    NombreMes = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy", new CultureInfo("es-ES")),
                    Completadas = g.Count(),
                    Totales = tareasTotales,
                    Porcentaje = tareasTotales > 0 ? Math.Round((decimal)g.Count() / tareasTotales * 100, 1) : 0
                })
                .OrderBy(x => x.Anio)
                .ThenBy(x => x.Mes)
                .ToList();

            return new ActividadesCompletadasDto
            {
                ActividadesTotales = actividadesGeneralesTotales,
                ActividadesCompletadas = completacionTotal,
                PorcentajeCompletacion = porcentajeCompletacion,
                TareasCompletadas = tareasCompletadas,
                TareasTotales = tareasTotales,
                CompletacionPorMes = completacionPorMes
            };
        }

        public async Task<TiempoDeUsoDto> ObtenerTiempoDeUsoAsync()
        {
            var hoy = DateTime.UtcNow.Date;
            var inicioMes = DateTime.SpecifyKind(new DateTime(hoy.Year, hoy.Month, 1), DateTimeKind.Utc);

            // Usuarios totales
            var usuariosTotales = await _db.Usuarios.CountAsync();

            // Usuarios que han iniciado sesión hoy (aproximación usando ActualizadoEn)
            var usuariosActivosHoy = await _db.Usuarios
                .CountAsync(u => u.EstaActivo && u.ActualizadoEn.HasValue && u.ActualizadoEn.Value.Date == hoy);

            // Si no hay datos de ActualizadoEn, usar usuarios activos en general
            if (usuariosActivosHoy == 0)
            {
                usuariosActivosHoy = await _db.Usuarios.CountAsync(u => u.EstaActivo);
            }

            // Porcentaje de usuarios activos
            var porcentajeActivos = usuariosTotales > 0
                ? Math.Round((decimal)usuariosActivosHoy / usuariosTotales * 100, 1)
                : 0;

            // Calcular tiempo promedio basado en actividad
            // Esto es una aproximación - en producción usarías una tabla de sesiones
            var actividadesEsteMes = await _db.Tareas
                .Where(t => t.FechaInicio >= inicioMes)
                .CountAsync();

            var asistenciasEsteMes = await _db.Asistencias
                .Where(a => a.Fecha >= inicioMes)
                .CountAsync();

            // Estimación: cada actividad/asistencia representa ~30 minutos
            var horasTotalesEstimadas = (actividadesEsteMes + asistenciasEsteMes) * 0.5m;
            var diasDelMes = (hoy - inicioMes).Days + 1;
            var promedioHorasDiarias = diasDelMes > 0 && usuariosActivosHoy > 0
                ? Math.Round(horasTotalesEstimadas / diasDelMes, 1)
                : 4.2m; // Valor por defecto

            return new TiempoDeUsoDto
            {
                PromedioHorasDiarias = promedioHorasDiarias,
                PorcentajeUsuariosActivos = porcentajeActivos,
                UsuariosActivosHoy = usuariosActivosHoy,
                UsuariosTotales = usuariosTotales,
                SesionesTotales = actividadesEsteMes + asistenciasEsteMes,
                TiempoTotalHoras = horasTotalesEstimadas
            };
        }

        public async Task<EstadisticasDetalladasDto> ObtenerEstadisticasDetalladasAsync()
        {
            var hoy = DateTime.UtcNow.Date;
            var inicioMes = DateTime.SpecifyKind(new DateTime(hoy.Year, hoy.Month, 1), DateTimeKind.Utc);

            // Usuarios
            var usuariosTotales = await _db.Usuarios.CountAsync();
            var usuariosActivos = await _db.Usuarios.CountAsync(u => u.EstaActivo);
            var usuariosInactivos = usuariosTotales - usuariosActivos;

            // Usuarios por rol
            var usuariosPorRol = await _db.Usuarios
                .Include(u => u.Rol)
                .GroupBy(u => new { u.IdRol, u.Rol.Nombre })
                .Select(g => new MetricasPorRolDto
                {
                    NombreRol = g.Key.Nombre,
                    CantidadUsuarios = g.Count(),
                    UsuariosActivos = g.Count(u => u.EstaActivo),
                    PorcentajeActivos = g.Count() > 0 
                        ? Math.Round((decimal)g.Count(u => u.EstaActivo) / g.Count() * 100, 1) 
                        : 0
                })
                .ToListAsync();

            // Niveles
            var nivelesActivos = await _db.Niveles.CountAsync(n => n.EstaActivo);
            var participantesPorNivel = await _db.NivelesDeParticipantes
                .Where(np => np.EstaActivo)
                .GroupBy(np => np.IdNivel)
                .Select(g => g.Count())
                .ToListAsync();

            var promedioParticipantes = participantesPorNivel.Any()
                ? Math.Round((decimal)participantesPorNivel.Average(), 1)
                : 0;

            // Actividades
            var actividadesActivas = await _db.Actividades.CountAsync(a => a.EstaActivo);
            var actividadesMontessori = await _db.ActividadesMontessori.CountAsync(a => a.EstaActivo);
            var actividadesTotales = actividadesActivas + actividadesMontessori;

            // Tareas
            var tareasTotales = await _db.Tareas.CountAsync(t => t.EstaActivo);
            var tareasCompletadas = await _db.Tareas.CountAsync(t => t.Estado == EstadoTarea.Completada || t.Estado == EstadoTarea.Aprobada);
            var tareasPendientes = await _db.Tareas.CountAsync(t => t.Estado == EstadoTarea.Pendiente);
            var tareasEnProceso = await _db.Tareas.CountAsync(t => t.Estado == EstadoTarea.EnProceso);

            // Asistencia
            var asistenciaHoy = await _db.Asistencias.CountAsync(a => a.Fecha.Date == hoy);
            var asistenciaMes = await _db.Asistencias.CountAsync(a => a.Fecha >= inicioMes);
            
            var diasDelMes = (hoy - inicioMes).Days + 1;
            var asistenciaEsperada = usuariosActivos * diasDelMes;
            var porcentajeAsistencia = asistenciaEsperada > 0
                ? Math.Round((decimal)asistenciaMes / asistenciaEsperada * 100, 1)
                : 0;

            return new EstadisticasDetalladasDto
            {
                TotalUsuarios = usuariosTotales,
                UsuariosActivos = usuariosActivos,
                UsuariosInactivos = usuariosInactivos,
                UsuariosPorRol = usuariosPorRol,
                TotalNiveles = await _db.Niveles.CountAsync(),
                NivelesActivos = nivelesActivos,
                PromedioParticipantesPorNivel = promedioParticipantes,
                TotalActividades = actividadesTotales,
                ActividadesActivas = actividadesActivas,
                ActividadesMontessori = actividadesMontessori,
                TotalTareas = tareasTotales,
                TareasCompletadas = tareasCompletadas,
                TareasPendientes = tareasPendientes,
                TareasEnProceso = tareasEnProceso,
                RegistrosAsistenciaHoy = asistenciaHoy,
                RegistrosAsistenciaEsteMes = asistenciaMes,
                PorcentajeAsistencia = porcentajeAsistencia
            };
        }
    }
}
