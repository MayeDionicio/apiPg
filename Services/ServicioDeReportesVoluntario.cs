using ApiPG.Data;
using ApiPG.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ApiPG.Services
{
    public interface IServicioDeReportesVoluntario
    {
        Task<ReporteVoluntarioDto?> GenerarReporteDeVoluntarioAsync(int idVoluntario, FiltrosReporteVoluntarioDto? filtros = null);
        Task<EstadisticasAsistenciaVoluntarioDto?> ObtenerEstadisticasAsistenciaAsync(int idVoluntario);
        Task<List<ParticipanteDelNivelDto>> ObtenerParticipantesConAsistenciaAsync(int idVoluntario);
        Task<List<AsistenciaDetalladaVoluntarioDto>> ObtenerAsistenciasDetalladasAsync(int idVoluntario, FiltrosReporteVoluntarioDto? filtros = null);
    }

    public class ServicioDeReportesVoluntario : IServicioDeReportesVoluntario
    {
        private readonly ApiPGContext _db;

        public ServicioDeReportesVoluntario(ApiPGContext db)
        {
            _db = db;
        }

        public async Task<ReporteVoluntarioDto?> GenerarReporteDeVoluntarioAsync(int idVoluntario, FiltrosReporteVoluntarioDto? filtros = null)
        {
            // Obtener el nivel asignado al voluntario
            var nivel = await _db.Niveles
                .Include(n => n.ParticipantesDelNivel)
                .FirstOrDefaultAsync(n => n.IdVoluntario == idVoluntario && n.EstaActivo);

            if (nivel == null)
            {
                return null; // El voluntario no tiene un nivel asignado
            }

            var reporte = new ReporteVoluntarioDto
            {
                MiNivel = new InfoNivelVoluntarioDto
                {
                    IdNivel = nivel.Id,
                    NombreNivel = nivel.Nombre,
                    Descripcion = nivel.Descripcion,
                    EdadMinima = nivel.EdadMinima,
                    EdadMaxima = nivel.EdadMaxima,
                    TotalParticipantes = nivel.ParticipantesDelNivel.Count,
                    ParticipantesActivos = nivel.ParticipantesDelNivel.Count(p => p.EstaActivo)
                },
                Asistencia = await ObtenerEstadisticasAsistenciaAsync(idVoluntario) ?? new(),
                Participantes = await ObtenerParticipantesConAsistenciaAsync(idVoluntario),
                FechaGeneracion = DateTime.UtcNow
            };

            return reporte;
        }

        public async Task<EstadisticasAsistenciaVoluntarioDto?> ObtenerEstadisticasAsistenciaAsync(int idVoluntario)
        {
            // Obtener el nivel del voluntario
            var nivel = await _db.Niveles
                .FirstOrDefaultAsync(n => n.IdVoluntario == idVoluntario && n.EstaActivo);

            if (nivel == null)
            {
                return null;
            }

            var hoy = DateTime.UtcNow.Date;
            var inicioDeSemana = hoy.AddDays(-(int)hoy.DayOfWeek);
            var inicioDeMes = new DateTime(hoy.Year, hoy.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            // Asistencias del nivel
            var asistencias = await _db.Asistencias
                .Where(a => a.IdNivel == nivel.Id)
                .ToListAsync();

            // Hoy
            var asistenciasHoy = asistencias.Where(a => a.Fecha.Date == hoy).ToList();
            var presentesHoy = asistenciasHoy.Count(a => a.Presente);
            var porcentajeHoy = asistenciasHoy.Any() 
                ? Math.Round((decimal)presentesHoy / asistenciasHoy.Count * 100, 1) 
                : 0;

            // Esta semana
            var asistenciasSemana = asistencias.Where(a => a.Fecha >= inicioDeSemana).ToList();
            var presentesSemana = asistenciasSemana.Count(a => a.Presente);
            var porcentajeSemana = asistenciasSemana.Any() 
                ? Math.Round((decimal)presentesSemana / asistenciasSemana.Count * 100, 1) 
                : 0;

            // Este mes
            var asistenciasMes = asistencias.Where(a => a.Fecha >= inicioDeMes).ToList();
            var presentesMes = asistenciasMes.Count(a => a.Presente);
            var porcentajeMes = asistenciasMes.Any() 
                ? Math.Round((decimal)presentesMes / asistenciasMes.Count * 100, 1) 
                : 0;

            // Últimos 7 días
            var hace7Dias = hoy.AddDays(-7);
            var ultimosDias = asistencias
                .Where(a => a.Fecha >= hace7Dias)
                .GroupBy(a => a.Fecha.Date)
                .Select(g => new AsistenciaDiariaVoluntarioDto
                {
                    Fecha = g.Key,
                    TotalRegistros = g.Count(),
                    Presentes = g.Count(a => a.Presente),
                    Ausentes = g.Count(a => !a.Presente),
                    PorcentajeAsistencia = g.Any() 
                        ? Math.Round((decimal)g.Count(a => a.Presente) / g.Count() * 100, 1) 
                        : 0
                })
                .OrderBy(a => a.Fecha)
                .ToList();

            return new EstadisticasAsistenciaVoluntarioDto
            {
                TotalRegistrosHoy = asistenciasHoy.Count,
                PresentesHoy = presentesHoy,
                AusentesHoy = asistenciasHoy.Count - presentesHoy,
                PorcentajeAsistenciaHoy = porcentajeHoy,
                
                TotalRegistrosEstaSemana = asistenciasSemana.Count,
                PresentesEstaSemana = presentesSemana,
                PorcentajeAsistenciaEstaSemana = porcentajeSemana,
                
                TotalRegistrosEsteMes = asistenciasMes.Count,
                PresentesEsteMes = presentesMes,
                PorcentajeAsistenciaEsteMes = porcentajeMes,
                
                UltimosDias = ultimosDias
            };
        }

        public async Task<List<ParticipanteDelNivelDto>> ObtenerParticipantesConAsistenciaAsync(int idVoluntario)
        {
            // Obtener el nivel del voluntario
            var nivel = await _db.Niveles
                .Include(n => n.ParticipantesDelNivel)
                    .ThenInclude(np => np.Usuario)
                .FirstOrDefaultAsync(n => n.IdVoluntario == idVoluntario && n.EstaActivo);

            if (nivel == null)
            {
                return new List<ParticipanteDelNivelDto>();
            }

            var participantes = new List<ParticipanteDelNivelDto>();

            foreach (var np in nivel.ParticipantesDelNivel.Where(p => p.EstaActivo))
            {
                var participante = np.Usuario;
                
                // Calcular edad
                var edad = participante.FechaDeNacimiento.HasValue
                    ? DateTime.Today.Year - participante.FechaDeNacimiento.Value.Year
                    : 0;

                // Obtener estadísticas de asistencia del participante
                var asistencias = await _db.Asistencias
                    .Where(a => a.IdNivel == nivel.Id && a.IdUsuario == participante.Id)
                    .ToListAsync();

                var totalPresencias = asistencias.Count(a => a.Presente);
                var totalAusencias = asistencias.Count(a => !a.Presente);
                var porcentaje = asistencias.Any() 
                    ? Math.Round((decimal)totalPresencias / asistencias.Count * 100, 1) 
                    : 0;

                participantes.Add(new ParticipanteDelNivelDto
                {
                    IdUsuario = participante.Id,
                    NombreCompleto = participante.NombreCompleto,
                    Edad = edad,
                    EstaActivo = participante.EstaActivo,
                    TotalAsistencias = asistencias.Count,
                    TotalPresencias = totalPresencias,
                    TotalAusencias = totalAusencias,
                    PorcentajeAsistencia = porcentaje,
                    UltimaAsistencia = asistencias.Any() 
                        ? asistencias.Max(a => a.Fecha) 
                        : null
                });
            }

            return participantes.OrderBy(p => p.NombreCompleto).ToList();
        }

        public async Task<List<AsistenciaDetalladaVoluntarioDto>> ObtenerAsistenciasDetalladasAsync(
            int idVoluntario, 
            FiltrosReporteVoluntarioDto? filtros = null)
        {
            // Obtener el nivel del voluntario
            var nivel = await _db.Niveles
                .FirstOrDefaultAsync(n => n.IdVoluntario == idVoluntario && n.EstaActivo);

            if (nivel == null)
            {
                return new List<AsistenciaDetalladaVoluntarioDto>();
            }

            var query = _db.Asistencias
                .Include(a => a.Usuario)
                .Where(a => a.IdNivel == nivel.Id);

            // Aplicar filtros
            if (filtros != null)
            {
                if (filtros.FechaInicio.HasValue)
                {
                    var fechaInicio = DateTime.SpecifyKind(filtros.FechaInicio.Value.Date, DateTimeKind.Utc);
                    query = query.Where(a => a.Fecha >= fechaInicio);
                }

                if (filtros.FechaFin.HasValue)
                {
                    var fechaFin = DateTime.SpecifyKind(filtros.FechaFin.Value.Date, DateTimeKind.Utc);
                    query = query.Where(a => a.Fecha <= fechaFin);
                }

                if (filtros.IdParticipante.HasValue)
                {
                    query = query.Where(a => a.IdUsuario == filtros.IdParticipante.Value);
                }
            }

            var asistencias = await query
                .OrderByDescending(a => a.Fecha)
                .ThenBy(a => a.Usuario.PrimerNombre)
                .ToListAsync();

            return asistencias.Select(a => new AsistenciaDetalladaVoluntarioDto
            {
                IdAsistencia = a.Id,
                IdParticipante = a.IdUsuario,
                NombreParticipante = a.Usuario.NombreCompleto,
                Fecha = a.Fecha,
                Presente = a.Presente,
                Observaciones = a.Observaciones,
                CreadoEn = a.CreadoEn
            }).ToList();
        }
    }
}
