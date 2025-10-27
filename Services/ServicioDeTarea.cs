using ApiPG.Data;
using ApiPG.DTOs;
using ApiPG.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPG.Services
{
    public class ServicioDeTarea : IServicioDeTarea
    {
        private readonly ApiPGContext _context;

        public ServicioDeTarea(ApiPGContext context)
        {
            _context = context;
        }

        public async Task<TareaDto> CrearTareaAsync(CrearTareaDto dto, int coordinadorId)
        {
            // Validar que las fechas sean coherentes
            if (dto.FechaInicio.HasValue && dto.FechaFin.HasValue && dto.FechaInicio > dto.FechaFin)
                throw new ArgumentException("La fecha de inicio no puede ser posterior a la fecha fin");

            // Validar que los voluntarios existan
            if (dto.VoluntariosIds.Any())
            {
                var voluntariosExisten = await _context.Usuarios
                    .Where(u => dto.VoluntariosIds.Contains(u.Id) && u.EstaActivo)
                    .CountAsync();

                if (voluntariosExisten != dto.VoluntariosIds.Count)
                    throw new ArgumentException("Uno o más voluntarios no existen o están inactivos");
            }

            var tarea = new Tarea
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                FechaInicio = dto.FechaInicio.HasValue 
                    ? DateTime.SpecifyKind(dto.FechaInicio.Value, DateTimeKind.Utc) 
                    : (DateTime?)null,
                FechaFin = dto.FechaFin.HasValue 
                    ? DateTime.SpecifyKind(dto.FechaFin.Value, DateTimeKind.Utc) 
                    : (DateTime?)null,
                CreadoPorIdUsuario = coordinadorId,
                NotasCoordinador = dto.NotasCoordinador,
                Estado = EstadoTarea.Pendiente,
                CreadoEn = DateTime.UtcNow,
                EstaActivo = true
            };

            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();

            // Asignar voluntarios
            if (dto.VoluntariosIds.Any())
            {
                foreach (var voluntarioId in dto.VoluntariosIds)
                {
                    var asignacion = new AsignacionDeTarea
                    {
                        TareaId = tarea.Id,
                        UsuarioId = voluntarioId,
                        EstadoVoluntario = EstadoTarea.Pendiente,
                        AsignadoEn = DateTime.UtcNow,
                        EstaActivo = true
                    };
                    _context.AsignacionesDeTarea.Add(asignacion);
                }
                await _context.SaveChangesAsync();
            }

            return await ObtenerTareaPorIdAsync(tarea.Id) 
                ?? throw new Exception("Error al obtener la tarea creada");
        }

        public async Task<TareaDto?> ObtenerTareaPorIdAsync(int id)
        {
            var tarea = await _context.Tareas
                .Include(t => t.CreadoPor)
                .Include(t => t.AsignacionesDeTarea)
                    .ThenInclude(a => a.Usuario)
                .FirstOrDefaultAsync(t => t.Id == id && t.EstaActivo);

            return tarea != null ? MapToDto(tarea) : null;
        }

        public async Task<IEnumerable<TareaDto>> ObtenerTodasLasTareasAsync()
        {
            var tareas = await _context.Tareas
                .Include(t => t.CreadoPor)
                .Include(t => t.AsignacionesDeTarea)
                    .ThenInclude(a => a.Usuario)
                .Where(t => t.EstaActivo)
                .OrderByDescending(t => t.CreadoEn)
                .ToListAsync();

            return tareas.Select(MapToDto);
        }

        public async Task<IEnumerable<TareaDto>> ObtenerTareasPorCoordinadorAsync(int coordinadorId)
        {
            var tareas = await _context.Tareas
                .Include(t => t.CreadoPor)
                .Include(t => t.AsignacionesDeTarea)
                    .ThenInclude(a => a.Usuario)
                .Where(t => t.CreadoPorIdUsuario == coordinadorId && t.EstaActivo)
                .OrderByDescending(t => t.CreadoEn)
                .ToListAsync();

            return tareas.Select(MapToDto);
        }

        public async Task<IEnumerable<TareaDto>> ObtenerTareasDeVoluntarioAsync(int voluntarioId)
        {
            var tareas = await _context.Tareas
                .Include(t => t.CreadoPor)
                .Include(t => t.AsignacionesDeTarea)
                    .ThenInclude(a => a.Usuario)
                .Where(t => t.AsignacionesDeTarea.Any(a => a.UsuarioId == voluntarioId && a.EstaActivo) 
                         && t.EstaActivo)
                .OrderByDescending(t => t.CreadoEn)
                .ToListAsync();

            return tareas.Select(MapToDto);
        }

        public async Task<TareaDto> ActualizarTareaAsync(int id, ActualizarTareaDto dto, int coordinadorId)
        {
            var tarea = await _context.Tareas
                .Include(t => t.AsignacionesDeTarea)
                .FirstOrDefaultAsync(t => t.Id == id && t.EstaActivo);

            if (tarea == null)
                throw new KeyNotFoundException("Tarea no encontrada");

            if (tarea.CreadoPorIdUsuario != coordinadorId)
                throw new UnauthorizedAccessException("Solo el coordinador que creó la tarea puede actualizarla");

            // Actualizar campos
            if (dto.Titulo != null) tarea.Titulo = dto.Titulo;
            if (dto.Descripcion != null) tarea.Descripcion = dto.Descripcion;
            if (dto.FechaInicio.HasValue) 
                tarea.FechaInicio = DateTime.SpecifyKind(dto.FechaInicio.Value, DateTimeKind.Utc);
            if (dto.FechaFin.HasValue) 
                tarea.FechaFin = DateTime.SpecifyKind(dto.FechaFin.Value, DateTimeKind.Utc);
            if (dto.NotasCoordinador != null) tarea.NotasCoordinador = dto.NotasCoordinador;

            tarea.ActualizadoEn = DateTime.UtcNow;

            // Actualizar voluntarios asignados si se proporciona la lista
            if (dto.VoluntariosIds != null)
            {
                // Remover asignaciones antiguas
                var asignacionesActuales = tarea.AsignacionesDeTarea.ToList();
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
                        var nuevaAsignacion = new AsignacionDeTarea
                        {
                            TareaId = tarea.Id,
                            UsuarioId = voluntarioId,
                            EstadoVoluntario = EstadoTarea.Pendiente,
                            AsignadoEn = DateTime.UtcNow,
                            EstaActivo = true
                        };
                        _context.AsignacionesDeTarea.Add(nuevaAsignacion);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return await ObtenerTareaPorIdAsync(id) 
                ?? throw new Exception("Error al obtener la tarea actualizada");
        }

        public async Task<bool> EliminarTareaAsync(int id, int coordinadorId)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            
            if (tarea == null || !tarea.EstaActivo)
                return false;

            if (tarea.CreadoPorIdUsuario != coordinadorId)
                throw new UnauthorizedAccessException("Solo el coordinador que creó la tarea puede eliminarla");

            tarea.EstaActivo = false;
            tarea.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TareaDto> ActualizarEstadoTareaAsync(int tareaId, int voluntarioId, ActualizarEstadoTareaDto dto)
        {
            var asignacion = await _context.AsignacionesDeTarea
                .Include(a => a.Tarea)
                .FirstOrDefaultAsync(a => a.TareaId == tareaId 
                                       && a.UsuarioId == voluntarioId 
                                       && a.EstaActivo);

            if (asignacion == null)
                throw new KeyNotFoundException("Asignación no encontrada");

            if (asignacion.Tarea == null)
                throw new InvalidOperationException("La tarea asociada no fue encontrada");

            var ahora = DateTime.UtcNow;

            // Actualizar estado según el flujo
            switch (dto.Estado)
            {
                case EstadoTarea.Vista:
                    if (asignacion.EstadoVoluntario == EstadoTarea.Pendiente)
                    {
                        asignacion.EstadoVoluntario = EstadoTarea.Vista;
                        asignacion.VistaEn = ahora;
                    }
                    break;

                case EstadoTarea.EnProceso:
                    asignacion.EstadoVoluntario = EstadoTarea.EnProceso;
                    if (!asignacion.IniciadaEn.HasValue)
                        asignacion.IniciadaEn = ahora;
                    if (!asignacion.VistaEn.HasValue)
                        asignacion.VistaEn = ahora;
                    break;

                case EstadoTarea.Completada:
                    asignacion.EstadoVoluntario = EstadoTarea.Completada;
                    asignacion.CompletadaEn = ahora;
                    if (!asignacion.IniciadaEn.HasValue)
                        asignacion.IniciadaEn = ahora;
                    if (!asignacion.VistaEn.HasValue)
                        asignacion.VistaEn = ahora;

                    // Si hay notas del voluntario, guardarlas en la tarea
                    if (!string.IsNullOrEmpty(dto.NotasVoluntario))
                    {
                        asignacion.Tarea.NotasVoluntario = dto.NotasVoluntario;
                    }

                    // Actualizar el estado general de la tarea si todos los voluntarios completaron
                    var todasCompletadas = await _context.AsignacionesDeTarea
                        .Where(a => a.TareaId == tareaId && a.EstaActivo)
                        .AllAsync(a => a.EstadoVoluntario == EstadoTarea.Completada 
                                    || a.EstadoVoluntario == EstadoTarea.Aprobada);

                    if (todasCompletadas)
                    {
                        asignacion.Tarea.Estado = EstadoTarea.Completada;
                    }
                    break;

                default:
                    throw new ArgumentException("Estado no válido para voluntario");
            }

            asignacion.Tarea.ActualizadoEn = ahora;
            await _context.SaveChangesAsync();

            return await ObtenerTareaPorIdAsync(tareaId) 
                ?? throw new Exception("Error al obtener la tarea actualizada");
        }

        public async Task<TareaDto> RevisarTareaAsync(int tareaId, RevisarTareaDto dto, int coordinadorId)
        {
            var tarea = await _context.Tareas
                .Include(t => t.AsignacionesDeTarea)
                .FirstOrDefaultAsync(t => t.Id == tareaId && t.EstaActivo);

            if (tarea == null)
                throw new KeyNotFoundException("Tarea no encontrada");

            if (tarea.CreadoPorIdUsuario != coordinadorId)
                throw new UnauthorizedAccessException("Solo el coordinador que creó la tarea puede revisarla");

            if (tarea.Estado != EstadoTarea.Completada)
                throw new InvalidOperationException("Solo se pueden revisar tareas completadas");

            var ahora = DateTime.UtcNow;

            if (dto.Aprobar)
            {
                tarea.Estado = EstadoTarea.Aprobada;
                tarea.FechaRevision = ahora;
                tarea.RazonRechazo = null;

                // Aprobar todas las asignaciones
                foreach (var asignacion in tarea.AsignacionesDeTarea.Where(a => a.EstaActivo))
                {
                    asignacion.EstadoVoluntario = EstadoTarea.Aprobada;
                }
            }
            else
            {
                tarea.Estado = EstadoTarea.Rechazada;
                tarea.FechaRevision = ahora;
                tarea.RazonRechazo = dto.RazonRechazo;

                // Regresar las asignaciones a EnProceso para que puedan corregir
                foreach (var asignacion in tarea.AsignacionesDeTarea.Where(a => a.EstaActivo))
                {
                    asignacion.EstadoVoluntario = EstadoTarea.EnProceso;
                }
            }

            tarea.ActualizadoEn = ahora;
            await _context.SaveChangesAsync();

            return await ObtenerTareaPorIdAsync(tareaId) 
                ?? throw new Exception("Error al obtener la tarea revisada");
        }

        public async Task<EstadisticasTareasDto> ObtenerEstadisticasAsync(int? coordinadorId = null, int? voluntarioId = null)
        {
            var query = _context.Tareas.Where(t => t.EstaActivo);

            if (coordinadorId.HasValue)
                query = query.Where(t => t.CreadoPorIdUsuario == coordinadorId.Value);

            if (voluntarioId.HasValue)
                query = query.Where(t => t.AsignacionesDeTarea.Any(a => a.UsuarioId == voluntarioId.Value && a.EstaActivo));

            var tareas = await query.ToListAsync();
            var total = tareas.Count;

            var estadisticas = new EstadisticasTareasDto
            {
                TotalTareas = total,
                TareasPendientes = tareas.Count(t => t.Estado == EstadoTarea.Pendiente || t.Estado == EstadoTarea.Vista),
                TareasEnProceso = tareas.Count(t => t.Estado == EstadoTarea.EnProceso),
                TareasCompletadas = tareas.Count(t => t.Estado == EstadoTarea.Completada),
                TareasAprobadas = tareas.Count(t => t.Estado == EstadoTarea.Aprobada),
                TareasRechazadas = tareas.Count(t => t.Estado == EstadoTarea.Rechazada),
                TareasVencidas = tareas.Count(t => t.FechaFin.HasValue 
                                                 && t.FechaFin.Value < DateTime.Today 
                                                 && t.Estado != EstadoTarea.Aprobada),
                PorcentajeCompletadas = total > 0 
                    ? Math.Round((double)tareas.Count(t => t.Estado == EstadoTarea.Aprobada) / total * 100, 2) 
                    : 0
            };

            return estadisticas;
        }

        private static TareaDto MapToDto(Tarea tarea)
        {
            return new TareaDto
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                FechaInicio = tarea.FechaInicio,
                FechaFin = tarea.FechaFin,
                CreadoPorIdUsuario = tarea.CreadoPorIdUsuario,
                NombreCoordinador = tarea.CreadoPor?.NombreCompleto ?? "Desconocido",
                Estado = tarea.Estado,
                EstadoTexto = ObtenerTextoEstado(tarea.Estado),
                Voluntarios = tarea.AsignacionesDeTarea
                    .Where(a => a.EstaActivo)
                    .Select(a => new AsignacionTareaDto
                    {
                        Id = a.Id,
                        UsuarioId = a.UsuarioId,
                        NombreVoluntario = a.Usuario?.NombreCompleto ?? "Desconocido",
                        EmailVoluntario = a.Usuario?.CorreoElectronico ?? "",
                        EstadoVoluntario = a.EstadoVoluntario,
                        AsignadoEn = a.AsignadoEn,
                        VistaEn = a.VistaEn,
                        IniciadaEn = a.IniciadaEn,
                        CompletadaEn = a.CompletadaEn
                    }).ToList(),
                NotasCoordinador = tarea.NotasCoordinador,
                NotasVoluntario = tarea.NotasVoluntario,
                CreadoEn = tarea.CreadoEn,
                ActualizadoEn = tarea.ActualizadoEn,
                FechaRevision = tarea.FechaRevision,
                RazonRechazo = tarea.RazonRechazo,
                EstaActivo = tarea.EstaActivo
            };
        }

        private static string ObtenerTextoEstado(EstadoTarea estado)
        {
            return estado switch
            {
                EstadoTarea.Pendiente => "Pendiente",
                EstadoTarea.Vista => "Vista",
                EstadoTarea.EnProceso => "En Proceso",
                EstadoTarea.Completada => "Completada",
                EstadoTarea.Aprobada => "Aprobada",
                EstadoTarea.Rechazada => "Rechazada",
                _ => "Desconocido"
            };
        }
    }
}
