using ApiPG.Data;
using ApiPG.DTOs;
using ApiPG.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPG.Services
{
    public interface IServicioDeReportes
    {
        Task<ReporteGeneralDto> GenerarReporteGeneralAsync(FiltrosReporteDto? filtros = null);
        Task<ResumenUsuariosDto> ObtenerResumenUsuariosAsync();
        Task<ResumenActividadesDto> ObtenerResumenActividadesAsync();
        Task<ResumenNivelesDto> ObtenerResumenNivelesAsync();
        Task<ResumenAsistenciaDto> ObtenerResumenAsistenciaAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null);
        Task<ResumenTareasDto> ObtenerResumenTareasAsync();
    }

    public class ServicioDeReportes : IServicioDeReportes
    {
        private readonly ApiPGContext _db;

        public ServicioDeReportes(ApiPGContext db)
        {
            _db = db;
        }

        public async Task<ReporteGeneralDto> GenerarReporteGeneralAsync(FiltrosReporteDto? filtros = null)
        {
            filtros ??= new FiltrosReporteDto();

            var reporte = new ReporteGeneralDto
            {
                Usuarios = await ObtenerResumenUsuariosAsync(),
                Actividades = await ObtenerResumenActividadesAsync(),
                Niveles = await ObtenerResumenNivelesAsync(),
                Asistencia = await ObtenerResumenAsistenciaAsync(filtros.FechaInicio, filtros.FechaFin),
                Tareas = await ObtenerResumenTareasAsync(),
                FechaGeneracion = DateTime.UtcNow
            };

            return reporte;
        }

        public async Task<ResumenUsuariosDto> ObtenerResumenUsuariosAsync()
        {
            var usuarios = await _db.Usuarios
                .Include(u => u.Rol)
                .ToListAsync();

            var totalUsuarios = usuarios.Count;
            var usuariosActivos = usuarios.Count(u => u.EstaActivo);
            var usuariosInactivos = totalUsuarios - usuariosActivos;

            var desglosePorRol = usuarios
                .GroupBy(u => new { u.IdRol, u.Rol.Nombre })
                .Select(g => new UsuariosPorRolDto
                {
                    NombreRol = g.Key.Nombre,
                    Cantidad = g.Count(),
                    Activos = g.Count(u => u.EstaActivo),
                    Inactivos = g.Count(u => !u.EstaActivo),
                    PorcentajeActivos = g.Count() > 0 
                        ? Math.Round((decimal)g.Count(u => u.EstaActivo) / g.Count() * 100, 1) 
                        : 0
                })
                .OrderBy(r => r.NombreRol)
                .ToList();

            // Contar por tipo de usuario
            var totalCoordinadores = usuarios.Count(u => u.Rol.Nombre.Contains("Coordinador", StringComparison.OrdinalIgnoreCase));
            var totalVoluntarios = usuarios.Count(u => u.Rol.Nombre.Contains("Voluntario", StringComparison.OrdinalIgnoreCase));
            var totalParticipantes = usuarios.Count(u => u.Rol.Nombre.Contains("Participante", StringComparison.OrdinalIgnoreCase));

            return new ResumenUsuariosDto
            {
                TotalUsuarios = totalUsuarios,
                TotalCoordinadores = totalCoordinadores,
                TotalVoluntarios = totalVoluntarios,
                TotalParticipantes = totalParticipantes,
                UsuariosActivos = usuariosActivos,
                UsuariosInactivos = usuariosInactivos,
                DesglosePorRol = desglosePorRol
            };
        }

        public async Task<ResumenActividadesDto> ObtenerResumenActividadesAsync()
        {
            var actividadesRegulares = await _db.Actividades
                .CountAsync(a => a.EstaActivo);

            var actividadesMontessori = await _db.ActividadesMontessori
                .ToListAsync();

            var totalMontessori = actividadesMontessori.Count;
            var montessoriActivas = actividadesMontessori.Count(a => a.EstaActivo);

            var logrosTotal = await _db.LogrosMontessori.CountAsync();
            var logrosObtenidos = await _db.LogrosMontessori.CountAsync(l => l.EsObtenido);

            var porcentajeLogros = logrosTotal > 0 
                ? Math.Round((decimal)logrosObtenidos / logrosTotal * 100, 1) 
                : 0;

            // Actividades por área pedagógica
            var actividadesPorArea = actividadesMontessori
                .Where(a => a.EstaActivo)
                .GroupBy(a => a.AreaPedagogica)
                .Select(g => new ActividadesPorAreaDto
                {
                    AreaPedagogica = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(a => a.Cantidad)
                .ToList();

            return new ResumenActividadesDto
            {
                TotalActividades = actividadesRegulares + totalMontessori,
                ActividadesRegulares = actividadesRegulares,
                ActividadesMontessori = totalMontessori,
                ActividadesActivas = actividadesRegulares + montessoriActivas,
                ActividadesInactivas = (totalMontessori - montessoriActivas),
                LogrosMontessoriObtenidos = logrosObtenidos,
                LogrosMontessoriTotales = logrosTotal,
                PorcentajeLogrosObtenidos = porcentajeLogros,
                ActividadesPorArea = actividadesPorArea
            };
        }

        public async Task<ResumenNivelesDto> ObtenerResumenNivelesAsync()
        {
            var niveles = await _db.Niveles
                .Include(n => n.Voluntario)
                .Include(n => n.ParticipantesDelNivel)
                .ToListAsync();

            var nivelesActivos = niveles.Where(n => n.EstaActivo).ToList();

            var desglosePorNivel = nivelesActivos
                .Select(n => new NivelConParticipantesDto
                {
                    IdNivel = n.Id,
                    NombreNivel = n.Nombre,
                    NombreVoluntario = n.Voluntario != null 
                        ? $"{n.Voluntario.PrimerNombre} {n.Voluntario.Apellido}" 
                        : "Sin asignar",
                    EdadMinima = n.EdadMinima,
                    EdadMaxima = n.EdadMaxima,
                    CantidadParticipantes = n.ParticipantesDelNivel.Count(p => p.EstaActivo)
                })
                .OrderBy(n => n.NombreNivel)
                .ToList();

            var totalParticipantes = desglosePorNivel.Sum(n => n.CantidadParticipantes);
            var promedio = nivelesActivos.Any() 
                ? Math.Round((decimal)totalParticipantes / nivelesActivos.Count, 1) 
                : 0;

            return new ResumenNivelesDto
            {
                TotalNiveles = niveles.Count,
                NivelesActivos = nivelesActivos.Count,
                TotalParticipantesAsignados = totalParticipantes,
                PromedioParticipantesPorNivel = promedio,
                DesglosePorNivel = desglosePorNivel
            };
        }

        public async Task<ResumenAsistenciaDto> ObtenerResumenAsistenciaAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            var hoy = DateTime.UtcNow.Date;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var inicioAnio = new DateTime(hoy.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var registrosHoy = await _db.Asistencias
                .CountAsync(a => a.Fecha.Date == hoy);

            var registrosMes = await _db.Asistencias
                .CountAsync(a => a.Fecha >= inicioMes);

            var registrosAnio = await _db.Asistencias
                .CountAsync(a => a.Fecha >= inicioAnio);

            // Últimos 7 días
            var hace7Dias = hoy.AddDays(-7);
            var asistenciaUltimos7Dias = await _db.Asistencias
                .Where(a => a.Fecha >= hace7Dias)
                .GroupBy(a => a.Fecha.Date)
                .Select(g => new AsistenciaPorDiaDto
                {
                    Fecha = g.Key,
                    CantidadPresentes = g.Count(a => a.Presente),
                    CantidadAusentes = g.Count(a => !a.Presente),
                    CantidadTardanzas = 0, // No hay campo Tardanza en el modelo actual
                    Total = g.Count()
                })
                .OrderBy(a => a.Fecha)
                .ToListAsync();

            // Calcular porcentaje de asistencia
            var totalRegistros = asistenciaUltimos7Dias.Sum(a => a.Total);
            var totalPresentes = asistenciaUltimos7Dias.Sum(a => a.CantidadPresentes);
            var porcentaje = totalRegistros > 0 
                ? Math.Round((decimal)totalPresentes / totalRegistros * 100, 1) 
                : 0;

            return new ResumenAsistenciaDto
            {
                TotalRegistrosHoy = registrosHoy,
                TotalRegistrosEsteMes = registrosMes,
                TotalRegistrosEsteAnio = registrosAnio,
                PorcentajeAsistenciaPromedio = porcentaje,
                AsistenciaUltimos7Dias = asistenciaUltimos7Dias
            };
        }

        public async Task<ResumenTareasDto> ObtenerResumenTareasAsync()
        {
            var tareas = await _db.Tareas
                .Include(t => t.AsignacionesDeTarea)
                    .ThenInclude(a => a.Usuario)
                .Where(t => t.EstaActivo)
                .ToListAsync();

            var totalTareas = tareas.Count;
            var pendientes = tareas.Count(t => t.Estado == EstadoTarea.Pendiente);
            var enProceso = tareas.Count(t => t.Estado == EstadoTarea.EnProceso);
            var completadas = tareas.Count(t => t.Estado == EstadoTarea.Completada);
            var aprobadas = tareas.Count(t => t.Estado == EstadoTarea.Aprobada);
            var rechazadas = tareas.Count(t => t.Estado == EstadoTarea.Rechazada);

            var porcentajeCompletadas = totalTareas > 0 
                ? Math.Round((decimal)(completadas + aprobadas) / totalTareas * 100, 1) 
                : 0;

            // Tareas por voluntario
            var tareasPorVoluntario = tareas
                .SelectMany(t => t.AsignacionesDeTarea)
                .Where(a => a.EstaActivo && a.Usuario != null)
                .GroupBy(a => new { a.UsuarioId, NombreVoluntario = $"{a.Usuario!.PrimerNombre} {a.Usuario.Apellido}" })
                .Select(g => new TareasPorVoluntarioDto
                {
                    IdVoluntario = g.Key.UsuarioId,
                    NombreVoluntario = g.Key.NombreVoluntario,
                    TareasAsignadas = g.Count(),
                    TareasCompletadas = g.Count(a => 
                        a.EstadoVoluntario == EstadoTarea.Completada || 
                        a.EstadoVoluntario == EstadoTarea.Aprobada),
                    PorcentajeCompletadas = g.Count() > 0 
                        ? Math.Round((decimal)g.Count(a => 
                            a.EstadoVoluntario == EstadoTarea.Completada || 
                            a.EstadoVoluntario == EstadoTarea.Aprobada) / g.Count() * 100, 1)
                        : 0
                })
                .OrderByDescending(v => v.TareasCompletadas)
                .ToList();

            return new ResumenTareasDto
            {
                TotalTareas = totalTareas,
                TareasPendientes = pendientes,
                TareasEnProceso = enProceso,
                TareasCompletadas = completadas,
                TareasAprobadas = aprobadas,
                TareasRechazadas = rechazadas,
                PorcentajeCompletadas = porcentajeCompletadas,
                TareasPorVoluntario = tareasPorVoluntario
            };
        }
    }
}
