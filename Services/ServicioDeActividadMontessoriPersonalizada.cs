using ApiPG.Data;
using ApiPG.DTOs;
using ApiPG.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ApiPG.Services
{
    public interface IServicioDeActividadMontessoriPersonalizada
    {
        Task<ActividadPersonalizadaDto> CrearActividadPersonalizadaAsync(CrearActividadPersonalizadaDto dto, int usuarioId);
        Task<ActividadPersonalizadaDto?> ObtenerActividadPorIdAsync(int id);
        Task<List<ActividadPersonalizadaDto>> ObtenerActividadesPorEstudianteAsync(int estudianteId, bool soloActivas = true);
        Task<List<ActividadPersonalizadaDto>> ObtenerActividadesConFiltrosAsync(FiltrosActividadPersonalizadaDto filtros);
        Task<ActividadPersonalizadaDto> ActualizarActividadAsync(int id, ActualizarActividadPersonalizadaDto dto);
        Task<ActividadPersonalizadaDto> ActualizarProgresoAsync(int id, ActualizarProgresoActividadDto dto);
        Task<bool> EliminarActividadAsync(int id);
        Task<EstadisticasActividadesEstudianteDto> ObtenerEstadisticasEstudianteAsync(int estudianteId);
        Task<List<ActividadPersonalizadaDto>> ObtenerActividadesPendientesAsync();
        Task<List<ActividadPersonalizadaDto>> ObtenerActividadesConRecordatorioAsync();
    }

    public class ServicioDeActividadMontessoriPersonalizada : IServicioDeActividadMontessoriPersonalizada
    {
        private readonly ApiPGContext _context;
        private readonly ILogger<ServicioDeActividadMontessoriPersonalizada> _logger;

        public ServicioDeActividadMontessoriPersonalizada(
            ApiPGContext context,
            ILogger<ServicioDeActividadMontessoriPersonalizada> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ActividadPersonalizadaDto> CrearActividadPersonalizadaAsync(
            CrearActividadPersonalizadaDto dto, 
            int usuarioId)
        {
            // Verificar que el estudiante existe
            var estudiante = await _context.Usuarios.FindAsync(dto.IdEstudiante);
            if (estudiante == null || !estudiante.EstaActivo)
            {
                throw new ArgumentException($"El estudiante con ID {dto.IdEstudiante} no existe o está inactivo");
            }

            // Verificar el nivel si se proporciona
            if (dto.IdNivel.HasValue)
            {
                var nivel = await _context.Niveles.FindAsync(dto.IdNivel.Value);
                if (nivel == null)
                {
                    throw new ArgumentException($"El nivel con ID {dto.IdNivel.Value} no existe");
                }
            }

            // Verificar la actividad base si se proporciona
            if (dto.IdActividadMontessoriBase.HasValue)
            {
                var actividadBase = await _context.ActividadesMontessori.FindAsync(dto.IdActividadMontessoriBase.Value);
                if (actividadBase == null)
                {
                    throw new ArgumentException($"La actividad base con ID {dto.IdActividadMontessoriBase.Value} no existe");
                }
            }

            var actividad = new ActividadMontessoriPersonalizada
            {
                IdActividadMontessoriBase = dto.IdActividadMontessoriBase,
                IdEstudiante = dto.IdEstudiante,
                IdNivel = dto.IdNivel,
                Nombre = dto.Nombre,
                AreaPedagogica = dto.AreaPedagogica,
                FechaAsignacion = DateTime.UtcNow,
                FechaInicio = dto.FechaInicio,
                HoraActividad = dto.HoraActividad,
                DuracionMinutosEstimada = dto.DuracionMinutosEstimada,
                ObjetivoPersonalizado = dto.ObjetivoPersonalizado,
                MaterialesNecesarios = dto.MaterialesNecesarios ?? string.Empty,
                PresentacionPersonalizada = dto.PresentacionPersonalizada,
                AdaptacionesEspeciales = dto.AdaptacionesEspeciales,
                Estado = "Pendiente",
                Prioridad = dto.Prioridad,
                EnviarRecordatorio = dto.EnviarRecordatorio,
                FechaRecordatorio = dto.FechaRecordatorio,
                RecordatorioEnviado = false,
                EsRecurrente = dto.EsRecurrente,
                FrecuenciaRecurrencia = dto.FrecuenciaRecurrencia,
                FechaFinRecurrencia = dto.FechaFinRecurrencia,
                NotasAdicionales = dto.NotasAdicionales,
                AsignadoPorIdUsuario = usuarioId,
                CreadoEn = DateTime.UtcNow,
                EstaActivo = true
            };

            _context.ActividadesMontessoriPersonalizadas.Add(actividad);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Actividad personalizada creada: ID {actividad.Id} para estudiante {dto.IdEstudiante}");

            return await ObtenerActividadPorIdAsync(actividad.Id) 
                ?? throw new Exception("Error al recuperar la actividad creada");
        }

        public async Task<ActividadPersonalizadaDto?> ObtenerActividadPorIdAsync(int id)
        {
            var actividad = await _context.ActividadesMontessoriPersonalizadas
                .Include(a => a.ActividadBase)
                .Include(a => a.Estudiante)
                .Include(a => a.Nivel)
                .Include(a => a.AsignadoPor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actividad == null) return null;

            return MapearADto(actividad);
        }

        public async Task<List<ActividadPersonalizadaDto>> ObtenerActividadesPorEstudianteAsync(
            int estudianteId, 
            bool soloActivas = true)
        {
            var query = _context.ActividadesMontessoriPersonalizadas
                .Include(a => a.ActividadBase)
                .Include(a => a.Estudiante)
                .Include(a => a.Nivel)
                .Include(a => a.AsignadoPor)
                .Where(a => a.IdEstudiante == estudianteId);

            if (soloActivas)
            {
                query = query.Where(a => a.EstaActivo);
            }

            var actividades = await query
                .OrderByDescending(a => a.FechaAsignacion)
                .ToListAsync();

            return actividades.Select(MapearADto).ToList();
        }

        public async Task<List<ActividadPersonalizadaDto>> ObtenerActividadesConFiltrosAsync(
            FiltrosActividadPersonalizadaDto filtros)
        {
            var query = _context.ActividadesMontessoriPersonalizadas
                .Include(a => a.ActividadBase)
                .Include(a => a.Estudiante)
                .Include(a => a.Nivel)
                .Include(a => a.AsignadoPor)
                .AsQueryable();

            if (filtros.IdEstudiante.HasValue)
            {
                query = query.Where(a => a.IdEstudiante == filtros.IdEstudiante.Value);
            }

            if (filtros.IdNivel.HasValue)
            {
                query = query.Where(a => a.IdNivel == filtros.IdNivel.Value);
            }

            if (!string.IsNullOrEmpty(filtros.Estado))
            {
                query = query.Where(a => a.Estado == filtros.Estado);
            }

            if (!string.IsNullOrEmpty(filtros.AreaPedagogica))
            {
                query = query.Where(a => a.AreaPedagogica == filtros.AreaPedagogica);
            }

            if (!string.IsNullOrEmpty(filtros.Prioridad))
            {
                query = query.Where(a => a.Prioridad == filtros.Prioridad);
            }

            if (filtros.FechaDesde.HasValue)
            {
                query = query.Where(a => a.FechaAsignacion >= filtros.FechaDesde.Value);
            }

            if (filtros.FechaHasta.HasValue)
            {
                query = query.Where(a => a.FechaAsignacion <= filtros.FechaHasta.Value);
            }

            if (filtros.RequiereRefuerzo.HasValue)
            {
                query = query.Where(a => a.RequiereRefuerzo == filtros.RequiereRefuerzo.Value);
            }

            if (filtros.SoloActivas == true)
            {
                query = query.Where(a => a.EstaActivo);
            }

            var actividades = await query
                .OrderByDescending(a => a.FechaAsignacion)
                .Skip((filtros.Pagina - 1) * filtros.RegistrosPorPagina)
                .Take(filtros.RegistrosPorPagina)
                .ToListAsync();

            return actividades.Select(MapearADto).ToList();
        }

        public async Task<ActividadPersonalizadaDto> ActualizarActividadAsync(
            int id, 
            ActualizarActividadPersonalizadaDto dto)
        {
            var actividad = await _context.ActividadesMontessoriPersonalizadas.FindAsync(id);
            if (actividad == null)
            {
                throw new KeyNotFoundException($"Actividad personalizada con ID {id} no encontrada");
            }

            // Actualizar solo los campos proporcionados
            if (!string.IsNullOrEmpty(dto.Nombre))
                actividad.Nombre = dto.Nombre;

            if (dto.FechaInicio.HasValue)
                actividad.FechaInicio = dto.FechaInicio;

            if (dto.HoraActividad.HasValue)
                actividad.HoraActividad = dto.HoraActividad;

            if (dto.DuracionMinutosEstimada.HasValue)
                actividad.DuracionMinutosEstimada = dto.DuracionMinutosEstimada.Value;

            if (!string.IsNullOrEmpty(dto.ObjetivoPersonalizado))
                actividad.ObjetivoPersonalizado = dto.ObjetivoPersonalizado;

            if (dto.MaterialesNecesarios != null)
                actividad.MaterialesNecesarios = dto.MaterialesNecesarios;

            if (dto.PresentacionPersonalizada != null)
                actividad.PresentacionPersonalizada = dto.PresentacionPersonalizada;

            if (dto.AdaptacionesEspeciales != null)
                actividad.AdaptacionesEspeciales = dto.AdaptacionesEspeciales;

            if (!string.IsNullOrEmpty(dto.Prioridad))
                actividad.Prioridad = dto.Prioridad;

            if (dto.EnviarRecordatorio.HasValue)
                actividad.EnviarRecordatorio = dto.EnviarRecordatorio.Value;

            if (dto.FechaRecordatorio.HasValue)
                actividad.FechaRecordatorio = dto.FechaRecordatorio;

            if (dto.NotasAdicionales != null)
                actividad.NotasAdicionales = dto.NotasAdicionales;

            actividad.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Actividad personalizada actualizada: ID {id}");

            return await ObtenerActividadPorIdAsync(id)
                ?? throw new Exception("Error al recuperar la actividad actualizada");
        }

        public async Task<ActividadPersonalizadaDto> ActualizarProgresoAsync(
            int id, 
            ActualizarProgresoActividadDto dto)
        {
            var actividad = await _context.ActividadesMontessoriPersonalizadas.FindAsync(id);
            if (actividad == null)
            {
                throw new KeyNotFoundException($"Actividad personalizada con ID {id} no encontrada");
            }

            actividad.Estado = dto.Estado;
            actividad.ProgresosPorcentaje = dto.ProgresosPorcentaje;
            actividad.FechaCompletada = dto.FechaCompletada;
            actividad.DuracionMinutosReal = dto.DuracionMinutosReal;
            actividad.ObservacionesProfesor = dto.ObservacionesProfesor;
            
            // Serializar logros alcanzados
            if (dto.LogrosAlcanzados != null && dto.LogrosAlcanzados.Any())
            {
                actividad.LogrosAlcanzados = JsonSerializer.Serialize(dto.LogrosAlcanzados);
            }

            actividad.DificultadesEncontradas = dto.DificultadesEncontradas;
            actividad.SugerenciasParaSeguimiento = dto.SugerenciasParaSeguimiento;
            actividad.NivelDesempeno = dto.NivelDesempeno;

            if (dto.RequiereRefuerzo.HasValue)
                actividad.RequiereRefuerzo = dto.RequiereRefuerzo.Value;

            actividad.EvidenciaUrlFoto = dto.EvidenciaUrlFoto;
            actividad.EvidenciaUrlVideo = dto.EvidenciaUrlVideo;
            actividad.ActualizadoEn = DateTime.UtcNow;

            // Si la actividad se completó, marcarla con progreso 100%
            if (dto.Estado == "Completada" && !actividad.ProgresosPorcentaje.HasValue)
            {
                actividad.ProgresosPorcentaje = 100;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Progreso actualizado para actividad: ID {id}, Estado: {dto.Estado}");

            return await ObtenerActividadPorIdAsync(id)
                ?? throw new Exception("Error al recuperar la actividad actualizada");
        }

        public async Task<bool> EliminarActividadAsync(int id)
        {
            var actividad = await _context.ActividadesMontessoriPersonalizadas.FindAsync(id);
            if (actividad == null)
            {
                return false;
            }

            // Soft delete
            actividad.EstaActivo = false;
            actividad.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Actividad personalizada eliminada (soft delete): ID {id}");

            return true;
        }

        public async Task<EstadisticasActividadesEstudianteDto> ObtenerEstadisticasEstudianteAsync(int estudianteId)
        {
            var estudiante = await _context.Usuarios.FindAsync(estudianteId);
            if (estudiante == null)
            {
                throw new KeyNotFoundException($"Estudiante con ID {estudianteId} no encontrado");
            }

            var actividades = await _context.ActividadesMontessoriPersonalizadas
                .Where(a => a.IdEstudiante == estudianteId && a.EstaActivo)
                .ToListAsync();

            var estadisticas = new EstadisticasActividadesEstudianteDto
            {
                IdEstudiante = estudianteId,
                NombreEstudiante = $"{estudiante.PrimerNombre} {estudiante.Apellido}",
                TotalActividades = actividades.Count,
                ActividadesPendientes = actividades.Count(a => a.Estado == "Pendiente"),
                ActividadesEnProgreso = actividades.Count(a => a.Estado == "EnProgreso"),
                ActividadesCompletadas = actividades.Count(a => a.Estado == "Completada"),
                ActividadesCanceladas = actividades.Count(a => a.Estado == "Cancelada"),
                PromedioProgreso = actividades.Any(a => a.ProgresosPorcentaje.HasValue)
                    ? actividades.Where(a => a.ProgresosPorcentaje.HasValue)
                                 .Average(a => a.ProgresosPorcentaje!.Value)
                    : 0,
                ActividadesConRefuerzo = actividades.Count(a => a.RequiereRefuerzo),
                ActividadesPorArea = actividades.GroupBy(a => a.AreaPedagogica)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ActividadesPorNivelDesempeno = actividades
                    .Where(a => !string.IsNullOrEmpty(a.NivelDesempeno))
                    .GroupBy(a => a.NivelDesempeno!)
                    .ToDictionary(g => g.Key, g => g.Count()),
                UltimaActividadCompletada = actividades
                    .Where(a => a.Estado == "Completada" && a.FechaCompletada.HasValue)
                    .OrderByDescending(a => a.FechaCompletada)
                    .Select(a => a.FechaCompletada)
                    .FirstOrDefault(),
                TotalMinutosTrabajados = actividades
                    .Where(a => a.DuracionMinutosReal.HasValue)
                    .Sum(a => a.DuracionMinutosReal!.Value)
            };

            return estadisticas;
        }

        public async Task<List<ActividadPersonalizadaDto>> ObtenerActividadesPendientesAsync()
        {
            var actividades = await _context.ActividadesMontessoriPersonalizadas
                .Include(a => a.ActividadBase)
                .Include(a => a.Estudiante)
                .Include(a => a.Nivel)
                .Include(a => a.AsignadoPor)
                .Where(a => a.EstaActivo && (a.Estado == "Pendiente" || a.Estado == "EnProgreso"))
                .OrderBy(a => a.Prioridad == "Alta" ? 1 : a.Prioridad == "Normal" ? 2 : 3)
                .ThenBy(a => a.FechaAsignacion)
                .ToListAsync();

            return actividades.Select(MapearADto).ToList();
        }

        public async Task<List<ActividadPersonalizadaDto>> ObtenerActividadesConRecordatorioAsync()
        {
            var ahora = DateTime.UtcNow;
            var actividades = await _context.ActividadesMontessoriPersonalizadas
                .Include(a => a.ActividadBase)
                .Include(a => a.Estudiante)
                .Include(a => a.Nivel)
                .Include(a => a.AsignadoPor)
                .Where(a => a.EstaActivo 
                    && a.EnviarRecordatorio 
                    && !a.RecordatorioEnviado
                    && a.FechaRecordatorio.HasValue 
                    && a.FechaRecordatorio.Value <= ahora)
                .ToListAsync();

            return actividades.Select(MapearADto).ToList();
        }

        private ActividadPersonalizadaDto MapearADto(ActividadMontessoriPersonalizada actividad)
        {
            List<string>? logros = null;
            if (!string.IsNullOrEmpty(actividad.LogrosAlcanzados))
            {
                try
                {
                    logros = JsonSerializer.Deserialize<List<string>>(actividad.LogrosAlcanzados);
                }
                catch
                {
                    // Si falla la deserialización, ignorar
                }
            }

            return new ActividadPersonalizadaDto
            {
                Id = actividad.Id,
                IdActividadMontessoriBase = actividad.IdActividadMontessoriBase,
                NombreActividadBase = actividad.ActividadBase?.Nombre,
                IdEstudiante = actividad.IdEstudiante,
                NombreEstudiante = $"{actividad.Estudiante.PrimerNombre} {actividad.Estudiante.Apellido}",
                EmailEstudiante = actividad.Estudiante.CorreoElectronico,
                IdNivel = actividad.IdNivel,
                NombreNivel = actividad.Nivel?.Nombre,
                Nombre = actividad.Nombre,
                AreaPedagogica = actividad.AreaPedagogica,
                FechaAsignacion = actividad.FechaAsignacion,
                FechaInicio = actividad.FechaInicio,
                FechaCompletada = actividad.FechaCompletada,
                HoraActividad = actividad.HoraActividad,
                DuracionMinutosEstimada = actividad.DuracionMinutosEstimada,
                DuracionMinutosReal = actividad.DuracionMinutosReal,
                ObjetivoPersonalizado = actividad.ObjetivoPersonalizado,
                MaterialesNecesarios = actividad.MaterialesNecesarios,
                PresentacionPersonalizada = actividad.PresentacionPersonalizada,
                AdaptacionesEspeciales = actividad.AdaptacionesEspeciales,
                Estado = actividad.Estado,
                ProgresosPorcentaje = actividad.ProgresosPorcentaje,
                ObservacionesProfesor = actividad.ObservacionesProfesor,
                LogrosAlcanzados = logros,
                DificultadesEncontradas = actividad.DificultadesEncontradas,
                SugerenciasParaSeguimiento = actividad.SugerenciasParaSeguimiento,
                NivelDesempeno = actividad.NivelDesempeno,
                RequiereRefuerzo = actividad.RequiereRefuerzo,
                EvidenciaUrlFoto = actividad.EvidenciaUrlFoto,
                EvidenciaUrlVideo = actividad.EvidenciaUrlVideo,
                NotasAdicionales = actividad.NotasAdicionales,
                EnviarRecordatorio = actividad.EnviarRecordatorio,
                FechaRecordatorio = actividad.FechaRecordatorio,
                RecordatorioEnviado = actividad.RecordatorioEnviado,
                Prioridad = actividad.Prioridad,
                AsignadoPorIdUsuario = actividad.AsignadoPorIdUsuario,
                NombreAsignadoPor = actividad.AsignadoPor != null 
                    ? $"{actividad.AsignadoPor.PrimerNombre} {actividad.AsignadoPor.Apellido}"
                    : "Desconocido",
                CreadoEn = actividad.CreadoEn,
                ActualizadoEn = actividad.ActualizadoEn,
                EstaActivo = actividad.EstaActivo,
                EsRecurrente = actividad.EsRecurrente,
                FrecuenciaRecurrencia = actividad.FrecuenciaRecurrencia,
                FechaFinRecurrencia = actividad.FechaFinRecurrencia
            };
        }
    }
}
