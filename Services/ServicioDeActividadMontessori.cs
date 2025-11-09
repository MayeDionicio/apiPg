using ApiPG.Data;
using ApiPG.DTOs;
using ApiPG.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ApiPG.Services
{
    public class ServicioDeActividadMontessori : IServicioDeActividadMontessori
    {
        private readonly ApiPGContext _context;

        public ServicioDeActividadMontessori(ApiPGContext context)
        {
            _context = context;
        }

        // ============ ACTIVIDADES MONTESSORI ============

        public async Task<ActividadMontessoriDto> CrearActividadAsync(CrearActividadMontessoriDto dto, int voluntarioId)
        {
            // Validaciones
            if (dto.EdadMinima < 0 || dto.EdadMaxima > 12 || dto.EdadMinima > dto.EdadMaxima)
                throw new ArgumentException("El rango de edad debe estar entre 0 y 12 años, y la edad mínima debe ser menor que la máxima");

            if (dto.DuracionMinutos < 3 || dto.DuracionMinutos > 180)
                throw new ArgumentException("La duración debe estar entre 3 y 180 minutos");

            var actividad = new ActividadMontessori
            {
                Nombre = dto.Nombre,
                AreaPedagogica = dto.AreaPedagogica,
                FechaActividad = DateTime.SpecifyKind(dto.FechaActividad, DateTimeKind.Utc),
                HoraActividad = dto.HoraActividad,
                EdadMinima = dto.EdadMinima,
                EdadMaxima = dto.EdadMaxima,
                DuracionMinutos = dto.DuracionMinutos,
                ObjetivoEspecifico = dto.ObjetivoEspecifico,
                MaterialesNecesarios = dto.MaterialesNecesarios,
                MontajeAmbiente = dto.MontajeAmbiente,
                Prerequisitos = dto.Prerequisitos,
                PresentacionPasoAPaso = dto.PresentacionPasoAPaso,
                ControlDelError = dto.ControlDelError,
                AspectosAutonomia = dto.AspectosAutonomia != null ? JsonSerializer.Serialize(dto.AspectosAutonomia) : null,
                LimitesYNormas = dto.LimitesYNormas,
                IndicadoresDeLogro = dto.IndicadoresDeLogro,
                AdaptacionesVariaciones = dto.AdaptacionesVariaciones,
                NivelDificultad = dto.NivelDificultad,
                EvidenciaUrl = dto.EvidenciaUrl,
                ObservacionesAdicionales = dto.ObservacionesAdicionales,
                ChecklistMontessori = dto.ChecklistMontessori != null ? JsonSerializer.Serialize(dto.ChecklistMontessori) : null,
                CreadoPorIdUsuario = voluntarioId,
                CreadoEn = DateTime.UtcNow,
                EstaActivo = true
            };

            _context.ActividadesMontessori.Add(actividad);
            await _context.SaveChangesAsync();

            return await ObtenerActividadPorIdAsync(actividad.Id)
                ?? throw new Exception("Error al obtener la actividad creada");
        }

        public async Task<ActividadMontessoriDto?> ObtenerActividadPorIdAsync(int id)
        {
            var actividad = await _context.ActividadesMontessori
                .Include(a => a.CreadoPor)
                .Include(a => a.Logros)
                    .ThenInclude(l => l.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id && a.EstaActivo);

            return actividad != null ? MapToDto(actividad) : null;
        }

        public async Task<IEnumerable<ActividadMontessoriDto>> ObtenerTodasLasActividadesAsync()
        {
            var actividades = await _context.ActividadesMontessori
                .Include(a => a.CreadoPor)
                .Include(a => a.Logros)
                    .ThenInclude(l => l.Usuario)
                .Where(a => a.EstaActivo)
                .OrderByDescending(a => a.CreadoEn)
                .ToListAsync();

            return actividades.Select(MapToDto);
        }

        public async Task<IEnumerable<ActividadMontessoriDto>> ObtenerActividadesPorVoluntarioAsync(int voluntarioId)
        {
            var actividades = await _context.ActividadesMontessori
                .Include(a => a.CreadoPor)
                .Include(a => a.Logros)
                    .ThenInclude(l => l.Usuario)
                .Where(a => a.CreadoPorIdUsuario == voluntarioId && a.EstaActivo)
                .OrderByDescending(a => a.CreadoEn)
                .ToListAsync();

            return actividades.Select(MapToDto);
        }

        public async Task<IEnumerable<ActividadMontessoriDto>> ObtenerActividadesPorAreaAsync(string area)
        {
            var actividades = await _context.ActividadesMontessori
                .Include(a => a.CreadoPor)
                .Include(a => a.Logros)
                    .ThenInclude(l => l.Usuario)
                .Where(a => a.AreaPedagogica.ToLower() == area.ToLower() && a.EstaActivo)
                .OrderByDescending(a => a.CreadoEn)
                .ToListAsync();

            return actividades.Select(MapToDto);
        }

        public async Task<IEnumerable<ActividadMontessoriDto>> ObtenerActividadesParaParticipanteAsync(int participanteId)
        {
            // Obtener el participante con su fecha de nacimiento
            var participante = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == participanteId && u.EstaActivo);

            if (participante == null)
                throw new KeyNotFoundException("Participante no encontrado");

            if (!participante.FechaDeNacimiento.HasValue)
                throw new ArgumentException("El participante no tiene fecha de nacimiento registrada");

            // Calcular la edad del participante
            var hoy = DateTime.Today;
            var edad = hoy.Year - participante.FechaDeNacimiento.Value.Year;
            if (participante.FechaDeNacimiento.Value.Date > hoy.AddYears(-edad))
                edad--;

            var edadDecimal = (decimal)edad;

            // Obtener actividades donde la edad del participante esté dentro del rango
            var actividades = await _context.ActividadesMontessori
                .Include(a => a.CreadoPor)
                .Include(a => a.Logros)
                    .ThenInclude(l => l.Usuario)
                .Where(a => a.EstaActivo 
                         && a.EdadMinima <= edadDecimal 
                         && a.EdadMaxima >= edadDecimal)
                .OrderByDescending(a => a.CreadoEn)
                .ToListAsync();

            return actividades.Select(MapToDto);
        }

        public async Task<IEnumerable<ActividadMontessoriDto>> ObtenerActividadesPorNivelAsync(int nivelId)
        {
            // Obtener el nivel con sus participantes
            var nivel = await _context.Niveles
                .Include(n => n.ParticipantesDelNivel)
                    .ThenInclude(np => np.Usuario)
                .FirstOrDefaultAsync(n => n.Id == nivelId && n.EstaActivo);

            if (nivel == null)
                throw new KeyNotFoundException("Nivel no encontrado");

            // Obtener la edad mínima y máxima de los participantes del nivel
            var participantesActivos = nivel.ParticipantesDelNivel
                .Where(np => np.EstaActivo && np.Usuario != null && np.Usuario.FechaDeNacimiento.HasValue)
                .ToList();

            if (!participantesActivos.Any())
            {
                // Si no hay participantes con edad, devolver actividades para el rango del nivel
                var actividades = await _context.ActividadesMontessori
                    .Include(a => a.CreadoPor)
                    .Include(a => a.Logros)
                        .ThenInclude(l => l.Usuario)
                    .Where(a => a.EstaActivo 
                             && a.EdadMinima <= nivel.EdadMaxima 
                             && a.EdadMaxima >= nivel.EdadMinima)
                    .OrderByDescending(a => a.CreadoEn)
                    .ToListAsync();

                return actividades.Select(MapToDto);
            }

            // Calcular edades de participantes
            var hoy = DateTime.Today;
            var edades = participantesActivos.Select(p =>
            {
                var edad = hoy.Year - p.Usuario!.FechaDeNacimiento!.Value.Year;
                if (p.Usuario.FechaDeNacimiento.Value.Date > hoy.AddYears(-edad))
                    edad--;
                return (decimal)edad;
            }).ToList();

            var edadMinima = edades.Min();
            var edadMaxima = edades.Max();

            // Obtener actividades que se ajusten al rango de edades del nivel
            var actividadesFiltradas = await _context.ActividadesMontessori
                .Include(a => a.CreadoPor)
                .Include(a => a.Logros)
                    .ThenInclude(l => l.Usuario)
                .Where(a => a.EstaActivo 
                         && ((a.EdadMinima <= edadMaxima && a.EdadMaxima >= edadMinima) // Actividad se solapa con rango del nivel
                            || (a.EdadMinima >= edadMinima && a.EdadMaxima <= edadMaxima))) // Actividad está dentro del rango
                .OrderByDescending(a => a.CreadoEn)
                .ToListAsync();

            return actividadesFiltradas.Select(MapToDto);
        }

        public async Task<ActividadMontessoriDto> ActualizarActividadAsync(int id, ActualizarActividadMontessoriDto dto, int voluntarioId)
        {
            var actividad = await _context.ActividadesMontessori
                .FirstOrDefaultAsync(a => a.Id == id && a.EstaActivo);

            if (actividad == null)
                throw new KeyNotFoundException("Actividad no encontrada");

            if (actividad.CreadoPorIdUsuario != voluntarioId)
                throw new UnauthorizedAccessException("Solo el voluntario que creó la actividad puede actualizarla");

            // Actualizar campos
            if (dto.Nombre != null) actividad.Nombre = dto.Nombre;
            if (dto.AreaPedagogica != null) actividad.AreaPedagogica = dto.AreaPedagogica;
            if (dto.FechaActividad.HasValue)
                actividad.FechaActividad = DateTime.SpecifyKind(dto.FechaActividad.Value, DateTimeKind.Utc);
            if (dto.HoraActividad.HasValue) actividad.HoraActividad = dto.HoraActividad;
            if (dto.EdadMinima.HasValue) actividad.EdadMinima = dto.EdadMinima.Value;
            if (dto.EdadMaxima.HasValue) actividad.EdadMaxima = dto.EdadMaxima.Value;
            if (dto.DuracionMinutos.HasValue) actividad.DuracionMinutos = dto.DuracionMinutos.Value;
            if (dto.ObjetivoEspecifico != null) actividad.ObjetivoEspecifico = dto.ObjetivoEspecifico;
            if (dto.MaterialesNecesarios != null) actividad.MaterialesNecesarios = dto.MaterialesNecesarios;
            if (dto.MontajeAmbiente != null) actividad.MontajeAmbiente = dto.MontajeAmbiente;
            if (dto.Prerequisitos != null) actividad.Prerequisitos = dto.Prerequisitos;
            if (dto.PresentacionPasoAPaso != null) actividad.PresentacionPasoAPaso = dto.PresentacionPasoAPaso;
            if (dto.ControlDelError != null) actividad.ControlDelError = dto.ControlDelError;
            if (dto.AspectosAutonomia != null)
                actividad.AspectosAutonomia = JsonSerializer.Serialize(dto.AspectosAutonomia);
            if (dto.LimitesYNormas != null) actividad.LimitesYNormas = dto.LimitesYNormas;
            if (dto.IndicadoresDeLogro != null) actividad.IndicadoresDeLogro = dto.IndicadoresDeLogro;
            if (dto.AdaptacionesVariaciones != null) actividad.AdaptacionesVariaciones = dto.AdaptacionesVariaciones;
            if (dto.NivelDificultad != null) actividad.NivelDificultad = dto.NivelDificultad;
            if (dto.EvidenciaUrl != null) actividad.EvidenciaUrl = dto.EvidenciaUrl;
            if (dto.ObservacionesAdicionales != null) actividad.ObservacionesAdicionales = dto.ObservacionesAdicionales;
            if (dto.ChecklistMontessori != null)
                actividad.ChecklistMontessori = JsonSerializer.Serialize(dto.ChecklistMontessori);

            actividad.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await ObtenerActividadPorIdAsync(id)
                ?? throw new Exception("Error al obtener la actividad actualizada");
        }

        public async Task<bool> EliminarActividadAsync(int id, int voluntarioId)
        {
            var actividad = await _context.ActividadesMontessori.FindAsync(id);

            if (actividad == null || !actividad.EstaActivo)
                return false;

            if (actividad.CreadoPorIdUsuario != voluntarioId)
                throw new UnauthorizedAccessException("Solo el voluntario que creó la actividad puede eliminarla");

            actividad.EstaActivo = false;
            actividad.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EstadisticasActividadesMontessoriDto> ObtenerEstadisticasAsync(int? voluntarioId = null)
        {
            var query = _context.ActividadesMontessori
                .Include(a => a.Logros)
                .Where(a => a.EstaActivo);

            if (voluntarioId.HasValue)
                query = query.Where(a => a.CreadoPorIdUsuario == voluntarioId.Value);

            var actividades = await query.ToListAsync();
            var total = actividades.Count;

            var actividadesPorArea = actividades
                .GroupBy(a => a.AreaPedagogica)
                .ToDictionary(g => g.Key, g => g.Count());

            var estadisticas = new EstadisticasActividadesMontessoriDto
            {
                TotalActividades = total,
                ActividadesPorArea = actividadesPorArea.Count,
                LogrosObtenidos = actividades.Sum(a => a.Logros.Count(l => l.EsObtenido && l.EstaActivo)),
                LogrosEnProgreso = actividades.Sum(a => a.Logros.Count(l => !l.EsObtenido && l.EstaActivo)),
                PromedioEdadMinima = total > 0 ? Math.Round((double)actividades.Average(a => a.EdadMinima), 1) : 0,
                PromedioDuracion = total > 0 ? Math.Round(actividades.Average(a => a.DuracionMinutos), 1) : 0,
                ActividadesPorAreaDetalle = actividadesPorArea
            };

            return estadisticas;
        }

        // ============ LOGROS MONTESSORI ============

        public async Task<LogroMontessoriDto> CrearLogroAsync(CrearLogroMontessoriDto dto)
        {
            // Validar que la actividad existe
            var actividadExiste = await _context.ActividadesMontessori
                .AnyAsync(a => a.Id == dto.ActividadMontessoriId && a.EstaActivo);

            if (!actividadExiste)
                throw new ArgumentException("La actividad Montessori no existe");

            // Validar que el usuario existe
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.Id == dto.UsuarioId && u.EstaActivo);

            if (!usuarioExiste)
                throw new ArgumentException("El usuario no existe");

            var logro = new LogroMontessori
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Icono = dto.Icono,
                ActividadMontessoriId = dto.ActividadMontessoriId,
                UsuarioId = dto.UsuarioId,
                EsObtenido = false,
                CreadoEn = DateTime.UtcNow,
                EstaActivo = true
            };

            _context.LogrosMontessori.Add(logro);
            await _context.SaveChangesAsync();

            // Cargar las relaciones
            await _context.Entry(logro).Reference(l => l.Usuario).LoadAsync();

            return MapLogroToDto(logro);
        }

        public async Task<LogroMontessoriDto> MarcarLogroComoObtenidoAsync(int logroId)
        {
            var logro = await _context.LogrosMontessori
                .Include(l => l.Usuario)
                .FirstOrDefaultAsync(l => l.Id == logroId && l.EstaActivo);

            if (logro == null)
                throw new KeyNotFoundException("Logro no encontrado");

            if (logro.EsObtenido)
                throw new InvalidOperationException("El logro ya fue obtenido anteriormente");

            logro.EsObtenido = true;
            logro.FechaObtencion = DateTime.UtcNow;
            logro.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapLogroToDto(logro);
        }

        public async Task<IEnumerable<LogroMontessoriDto>> ObtenerLogrosPorActividadAsync(int actividadId)
        {
            var logros = await _context.LogrosMontessori
                .Include(l => l.Usuario)
                .Where(l => l.ActividadMontessoriId == actividadId && l.EstaActivo)
                .OrderByDescending(l => l.EsObtenido)
                .ThenByDescending(l => l.FechaObtencion)
                .ToListAsync();

            return logros.Select(MapLogroToDto);
        }

        public async Task<IEnumerable<LogroMontessoriDto>> ObtenerLogrosPorUsuarioAsync(int usuarioId)
        {
            var logros = await _context.LogrosMontessori
                .Include(l => l.Usuario)
                .Where(l => l.UsuarioId == usuarioId && l.EstaActivo)
                .OrderByDescending(l => l.EsObtenido)
                .ThenByDescending(l => l.FechaObtencion)
                .ToListAsync();

            return logros.Select(MapLogroToDto);
        }

        public async Task<bool> EliminarLogroAsync(int logroId)
        {
            var logro = await _context.LogrosMontessori.FindAsync(logroId);

            if (logro == null || !logro.EstaActivo)
                return false;

            logro.EstaActivo = false;
            logro.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // ============ MAPPERS ============

        private static ActividadMontessoriDto MapToDto(ActividadMontessori actividad)
        {
            return new ActividadMontessoriDto
            {
                Id = actividad.Id,
                Nombre = actividad.Nombre,
                AreaPedagogica = actividad.AreaPedagogica,
                FechaActividad = actividad.FechaActividad,
                HoraActividad = actividad.HoraActividad,
                EdadMinima = actividad.EdadMinima,
                EdadMaxima = actividad.EdadMaxima,
                DuracionMinutos = actividad.DuracionMinutos,
                ObjetivoEspecifico = actividad.ObjetivoEspecifico,
                MaterialesNecesarios = actividad.MaterialesNecesarios,
                MontajeAmbiente = actividad.MontajeAmbiente,
                Prerequisitos = actividad.Prerequisitos,
                PresentacionPasoAPaso = actividad.PresentacionPasoAPaso,
                ControlDelError = actividad.ControlDelError,
                AspectosAutonomia = !string.IsNullOrEmpty(actividad.AspectosAutonomia)
                    ? JsonSerializer.Deserialize<List<string>>(actividad.AspectosAutonomia)
                    : null,
                LimitesYNormas = actividad.LimitesYNormas,
                IndicadoresDeLogro = actividad.IndicadoresDeLogro,
                AdaptacionesVariaciones = actividad.AdaptacionesVariaciones,
                NivelDificultad = actividad.NivelDificultad,
                EvidenciaUrl = actividad.EvidenciaUrl,
                ObservacionesAdicionales = actividad.ObservacionesAdicionales,
                ChecklistMontessori = !string.IsNullOrEmpty(actividad.ChecklistMontessori)
                    ? JsonSerializer.Deserialize<List<string>>(actividad.ChecklistMontessori)
                    : null,
                CreadoPorIdUsuario = actividad.CreadoPorIdUsuario,
                NombreVoluntario = actividad.CreadoPor?.NombreCompleto ?? "Desconocido",
                Logros = actividad.Logros
                    .Where(l => l.EstaActivo)
                    .Select(MapLogroToDto)
                    .ToList(),
                CreadoEn = actividad.CreadoEn,
                ActualizadoEn = actividad.ActualizadoEn,
                EstaActivo = actividad.EstaActivo
            };
        }

        private static LogroMontessoriDto MapLogroToDto(LogroMontessori logro)
        {
            return new LogroMontessoriDto
            {
                Id = logro.Id,
                Nombre = logro.Nombre,
                Descripcion = logro.Descripcion,
                Icono = logro.Icono,
                EsObtenido = logro.EsObtenido,
                FechaObtencion = logro.FechaObtencion,
                ActividadMontessoriId = logro.ActividadMontessoriId,
                UsuarioId = logro.UsuarioId,
                NombreUsuario = logro.Usuario?.NombreCompleto ?? "Desconocido",
                CreadoEn = logro.CreadoEn,
                EstaActivo = logro.EstaActivo
            };
        }
    }
}
