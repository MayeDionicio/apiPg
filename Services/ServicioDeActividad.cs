using ApiPG.Data;
using ApiPG.DTOs;
using ApiPG.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPG.Services
{
    public class ServicioDeActividad : IServicioDeActividad
    {
        private readonly ApiPGContext _context;

        public ServicioDeActividad(ApiPGContext context)
        {
            _context = context;
        }

        public async Task<ActividadDto> CrearActividadAsync(CrearActividadDto dto, int coordinadorId)
        {
            // Validar que la fecha del evento no sea en el pasado
            if (dto.FechaDelEvento.Date < DateTime.Today)
                throw new ArgumentException("La fecha del evento no puede ser en el pasado");

            // Validar que los voluntarios existan
            if (dto.VoluntariosIds.Any())
            {
                var voluntariosExisten = await _context.Usuarios
                    .Where(u => dto.VoluntariosIds.Contains(u.Id) && u.EstaActivo)
                    .CountAsync();

                if (voluntariosExisten != dto.VoluntariosIds.Count)
                    throw new ArgumentException("Uno o más voluntarios no existen o están inactivos");
            }

            var actividad = new Actividad
            {
                Titulo = dto.Titulo,
                AreaDeEnfoque = dto.AreaDeEnfoque,
                DescripcionDetallada = dto.DescripcionDetallada,
                DuracionEstimadaMinutos = dto.DuracionEstimadaMinutos,
                MaterialesNecesarios = dto.MaterialesNecesarios,
                FechaDelEvento = DateTime.SpecifyKind(dto.FechaDelEvento, DateTimeKind.Utc),
                CreadoPorIdUsuario = coordinadorId,
                CreadoEn = DateTime.UtcNow,
                EstaActivo = true
            };

            _context.Actividades.Add(actividad);
            await _context.SaveChangesAsync();

            // Asignar voluntarios
            if (dto.VoluntariosIds.Any())
            {
                foreach (var voluntarioId in dto.VoluntariosIds)
                {
                    var asignacion = new AsignacionDeActividad
                    {
                        ActividadId = actividad.Id,
                        UsuarioId = voluntarioId,
                        AsignadoEn = DateTime.UtcNow,
                        EstaActivo = true
                    };
                    _context.AsignacionesDeActividad.Add(asignacion);
                }
                await _context.SaveChangesAsync();
            }

            return await ObtenerActividadPorIdAsync(actividad.Id) 
                ?? throw new Exception("Error al obtener la actividad creada");
        }

        public async Task<ActividadDto?> ObtenerActividadPorIdAsync(int id)
        {
            var actividad = await _context.Actividades
                .Include(a => a.CreadoPor)
                .Include(a => a.AsignacionesDeActividad)
                    .ThenInclude(aa => aa.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id && a.EstaActivo);

            return actividad != null ? MapToDto(actividad) : null;
        }

        public async Task<IEnumerable<ActividadDto>> ObtenerTodasLasActividadesAsync()
        {
            var actividades = await _context.Actividades
                .Include(a => a.CreadoPor)
                .Include(a => a.AsignacionesDeActividad)
                    .ThenInclude(aa => aa.Usuario)
                .Where(a => a.EstaActivo)
                .OrderBy(a => a.FechaDelEvento)
                .ToListAsync();

            return actividades.Select(MapToDto);
        }

        public async Task<IEnumerable<ActividadDto>> ObtenerActividadesPorCoordinadorAsync(int coordinadorId)
        {
            var actividades = await _context.Actividades
                .Include(a => a.CreadoPor)
                .Include(a => a.AsignacionesDeActividad)
                    .ThenInclude(aa => aa.Usuario)
                .Where(a => a.CreadoPorIdUsuario == coordinadorId && a.EstaActivo)
                .OrderBy(a => a.FechaDelEvento)
                .ToListAsync();

            return actividades.Select(MapToDto);
        }

        public async Task<IEnumerable<ActividadDto>> ObtenerActividadesDeVoluntarioAsync(int voluntarioId)
        {
            var actividades = await _context.Actividades
                .Include(a => a.CreadoPor)
                .Include(a => a.AsignacionesDeActividad)
                    .ThenInclude(aa => aa.Usuario)
                .Where(a => a.AsignacionesDeActividad.Any(aa => aa.UsuarioId == voluntarioId && aa.EstaActivo) 
                         && a.EstaActivo)
                .OrderBy(a => a.FechaDelEvento)
                .ToListAsync();

            return actividades.Select(MapToDto);
        }

        public async Task<IEnumerable<ActividadDto>> ObtenerActividadesPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var actividades = await _context.Actividades
                .Include(a => a.CreadoPor)
                .Include(a => a.AsignacionesDeActividad)
                    .ThenInclude(aa => aa.Usuario)
                .Where(a => a.FechaDelEvento.Date >= fechaInicio.Date 
                         && a.FechaDelEvento.Date <= fechaFin.Date 
                         && a.EstaActivo)
                .OrderBy(a => a.FechaDelEvento)
                .ToListAsync();

            return actividades.Select(MapToDto);
        }

        public async Task<ActividadDto> ActualizarActividadAsync(int id, ActualizarActividadDto dto, int coordinadorId)
        {
            var actividad = await _context.Actividades
                .Include(a => a.AsignacionesDeActividad)
                .FirstOrDefaultAsync(a => a.Id == id && a.EstaActivo);

            if (actividad == null)
                throw new KeyNotFoundException("Actividad no encontrada");

            if (actividad.CreadoPorIdUsuario != coordinadorId)
                throw new UnauthorizedAccessException("Solo el coordinador que creó la actividad puede actualizarla");

            // Actualizar campos
            if (dto.Titulo != null) actividad.Titulo = dto.Titulo;
            if (dto.AreaDeEnfoque != null) actividad.AreaDeEnfoque = dto.AreaDeEnfoque;
            if (dto.DescripcionDetallada != null) actividad.DescripcionDetallada = dto.DescripcionDetallada;
            if (dto.DuracionEstimadaMinutos.HasValue) actividad.DuracionEstimadaMinutos = dto.DuracionEstimadaMinutos.Value;
            if (dto.MaterialesNecesarios != null) actividad.MaterialesNecesarios = dto.MaterialesNecesarios;
            if (dto.FechaDelEvento.HasValue)
            {
                if (dto.FechaDelEvento.Value.Date < DateTime.Today)
                    throw new ArgumentException("La fecha del evento no puede ser en el pasado");
                actividad.FechaDelEvento = DateTime.SpecifyKind(dto.FechaDelEvento.Value, DateTimeKind.Utc);
            }

            actividad.ActualizadoEn = DateTime.UtcNow;

            // Actualizar voluntarios asignados si se proporciona la lista
            if (dto.VoluntariosIds != null)
            {
                // Remover asignaciones antiguas
                var asignacionesActuales = actividad.AsignacionesDeActividad.ToList();
                foreach (var asignacion in asignacionesActuales)
                {
                    if (!dto.VoluntariosIds.Contains(asignacion.UsuarioId))
                    {
                        asignacion.EstaActivo = false;
                    }
                }

                // Agregar nuevas asignaciones
                foreach (var voluntarioId in dto.VoluntariosIds)
                {
                    if (!asignacionesActuales.Any(a => a.UsuarioId == voluntarioId && a.EstaActivo))
                    {
                        var nuevaAsignacion = new AsignacionDeActividad
                        {
                            ActividadId = actividad.Id,
                            UsuarioId = voluntarioId,
                            AsignadoEn = DateTime.UtcNow,
                            EstaActivo = true
                        };
                        _context.AsignacionesDeActividad.Add(nuevaAsignacion);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return await ObtenerActividadPorIdAsync(id) 
                ?? throw new Exception("Error al obtener la actividad actualizada");
        }

        public async Task<bool> EliminarActividadAsync(int id, int coordinadorId)
        {
            var actividad = await _context.Actividades.FindAsync(id);
            
            if (actividad == null || !actividad.EstaActivo)
                return false;

            if (actividad.CreadoPorIdUsuario != coordinadorId)
                throw new UnauthorizedAccessException("Solo el coordinador que creó la actividad puede eliminarla");

            actividad.EstaActivo = false;
            actividad.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EstadisticasActividadesDto> ObtenerEstadisticasAsync(int? coordinadorId = null)
        {
            var query = _context.Actividades.Where(a => a.EstaActivo);

            if (coordinadorId.HasValue)
                query = query.Where(a => a.CreadoPorIdUsuario == coordinadorId.Value);

            var actividades = await query
                .Include(a => a.AsignacionesDeActividad)
                .ToListAsync();

            var total = actividades.Count;
            var ahora = DateTime.Today;
            var inicioMes = new DateTime(ahora.Year, ahora.Month, 1);
            var finMes = inicioMes.AddMonths(1).AddDays(-1);

            var estadisticas = new EstadisticasActividadesDto
            {
                TotalActividades = total,
                ActividadesPendientes = actividades.Count(a => a.FechaDelEvento.Date >= ahora),
                ActividadesRealizadas = actividades.Count(a => a.FechaDelEvento.Date < ahora),
                ActividadesEsteMes = actividades.Count(a => a.FechaDelEvento.Date >= inicioMes 
                                                          && a.FechaDelEvento.Date <= finMes),
                PromedioVoluntariosPorActividad = total > 0 
                    ? Math.Round((double)actividades.Sum(a => a.AsignacionesDeActividad.Count(aa => aa.EstaActivo)) / total, 2) 
                    : 0,
                PromedioDuracionMinutos = total > 0 
                    ? Math.Round((double)actividades.Sum(a => a.DuracionEstimadaMinutos) / total, 2) 
                    : 0
            };

            return estadisticas;
        }

        private static ActividadDto MapToDto(Actividad actividad)
        {
            return new ActividadDto
            {
                Id = actividad.Id,
                Titulo = actividad.Titulo,
                AreaDeEnfoque = actividad.AreaDeEnfoque,
                DescripcionDetallada = actividad.DescripcionDetallada,
                DuracionEstimadaMinutos = actividad.DuracionEstimadaMinutos,
                MaterialesNecesarios = actividad.MaterialesNecesarios,
                FechaDelEvento = actividad.FechaDelEvento,
                CreadoPorIdUsuario = actividad.CreadoPorIdUsuario,
                NombreCoordinador = actividad.CreadoPor?.NombreCompleto ?? "Desconocido",
                Voluntarios = actividad.AsignacionesDeActividad
                    .Where(aa => aa.EstaActivo)
                    .Select(aa => new VoluntarioAsignadoDto
                    {
                        Id = aa.Id,
                        UsuarioId = aa.UsuarioId,
                        NombreVoluntario = aa.Usuario?.NombreCompleto ?? "Desconocido",
                        EmailVoluntario = aa.Usuario?.CorreoElectronico ?? "",
                        AsignadoEn = aa.AsignadoEn
                    }).ToList(),
                CreadoEn = actividad.CreadoEn,
                ActualizadoEn = actividad.ActualizadoEn,
                EstaActivo = actividad.EstaActivo
            };
        }
    }
}
