using ApiPG.Data;
using ApiPG.DTOs;
using ApiPG.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPG.Services
{
    public interface IServicioDeLogrosFacilitador
    {
        // Gestión de logros (Coordinadores)
        Task<LogroFacilitadorDto> CrearLogroAsync(CrearLogroFacilitadorDto dto, int coordinadorId);
        Task<LogroFacilitadorDto> ActualizarLogroAsync(int id, ActualizarLogroFacilitadorDto dto);
        Task<bool> EliminarLogroAsync(int id);
        Task<LogroFacilitadorDto?> ObtenerLogroPorIdAsync(int id);
        Task<List<LogroFacilitadorDto>> ObtenerTodosLosLogrosAsync(FiltrosLogrosDto? filtros = null);
        
        // Otorgar logros (Coordinadores)
        Task<LogroObtenidoFacilitadorDto> OtorgarLogroAsync(OtorgarLogroDto dto, int coordinadorId);
        Task<bool> RevocarLogroAsync(int idLogroObtenido, int coordinadorId);
        
        // Consultar logros obtenidos
        Task<List<LogroObtenidoFacilitadorDto>> ObtenerLogrosObtenidosAsync(FiltrosLogrosObtenidosDto? filtros = null);
        Task<PerfilLogrosFacilitadorDto> ObtenerPerfilFacilitadorAsync(int facilitadorId);
        Task<List<RankingFacilitadorDto>> ObtenerRankingFacilitadoresAsync(int top = 10);
        
        // Estadísticas
        Task<EstadisticasLogrosDto> ObtenerEstadisticasGeneralesAsync();
    }

    public class ServicioDeLogrosFacilitador : IServicioDeLogrosFacilitador
    {
        private readonly ApiPGContext _context;
        private readonly ILogger<ServicioDeLogrosFacilitador> _logger;

        public ServicioDeLogrosFacilitador(
            ApiPGContext context,
            ILogger<ServicioDeLogrosFacilitador> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<LogroFacilitadorDto> CrearLogroAsync(CrearLogroFacilitadorDto dto, int coordinadorId)
        {
            var logro = new LogroFacilitador
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Categoria = dto.Categoria,
                Icono = dto.Icono,
                PuntosValor = dto.PuntosValor,
                CriteriosObtencion = dto.CriteriosObtencion,
                TipoLogro = dto.TipoLogro,
                CantidadActividadesRequeridas = dto.CantidadActividadesRequeridas,
                CantidadEstudiantesRequeridos = dto.CantidadEstudiantesRequeridos,
                DiasConsecutivosRequeridos = dto.DiasConsecutivosRequeridos,
                CreadoPorIdUsuario = coordinadorId,
                CreadoEn = DateTime.UtcNow,
                EstaActivo = true
            };

            _context.LogrosFacilitador.Add(logro);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Logro creado: {logro.Nombre} (ID: {logro.Id}) por coordinador {coordinadorId}");

            return await ObtenerLogroPorIdAsync(logro.Id) 
                ?? throw new Exception("Error al recuperar el logro creado");
        }

        public async Task<LogroFacilitadorDto> ActualizarLogroAsync(int id, ActualizarLogroFacilitadorDto dto)
        {
            var logro = await _context.LogrosFacilitador.FindAsync(id);
            if (logro == null)
            {
                throw new KeyNotFoundException($"Logro con ID {id} no encontrado");
            }

            if (!string.IsNullOrEmpty(dto.Nombre))
                logro.Nombre = dto.Nombre;

            if (!string.IsNullOrEmpty(dto.Descripcion))
                logro.Descripcion = dto.Descripcion;

            if (dto.Categoria != null)
                logro.Categoria = dto.Categoria;

            if (dto.Icono != null)
                logro.Icono = dto.Icono;

            if (dto.PuntosValor.HasValue)
                logro.PuntosValor = dto.PuntosValor.Value;

            if (dto.CriteriosObtencion != null)
                logro.CriteriosObtencion = dto.CriteriosObtencion;

            if (dto.CantidadActividadesRequeridas.HasValue)
                logro.CantidadActividadesRequeridas = dto.CantidadActividadesRequeridas;

            if (dto.CantidadEstudiantesRequeridos.HasValue)
                logro.CantidadEstudiantesRequeridos = dto.CantidadEstudiantesRequeridos;

            if (dto.DiasConsecutivosRequeridos.HasValue)
                logro.DiasConsecutivosRequeridos = dto.DiasConsecutivosRequeridos;

            logro.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Logro actualizado: ID {id}");

            return await ObtenerLogroPorIdAsync(id)
                ?? throw new Exception("Error al recuperar el logro actualizado");
        }

        public async Task<bool> EliminarLogroAsync(int id)
        {
            var logro = await _context.LogrosFacilitador.FindAsync(id);
            if (logro == null)
            {
                return false;
            }

            // Soft delete
            logro.EstaActivo = false;
            logro.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Logro eliminado (soft delete): ID {id}");

            return true;
        }

        public async Task<LogroFacilitadorDto?> ObtenerLogroPorIdAsync(int id)
        {
            var logro = await _context.LogrosFacilitador
                .Include(l => l.CreadoPor)
                .Include(l => l.LogrosObtenidos)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (logro == null) return null;

            return MapearLogroADto(logro);
        }

        public async Task<List<LogroFacilitadorDto>> ObtenerTodosLosLogrosAsync(FiltrosLogrosDto? filtros = null)
        {
            var query = _context.LogrosFacilitador
                .Include(l => l.CreadoPor)
                .Include(l => l.LogrosObtenidos)
                .AsQueryable();

            if (filtros != null)
            {
                if (!string.IsNullOrEmpty(filtros.Categoria))
                {
                    query = query.Where(l => l.Categoria == filtros.Categoria);
                }

                if (!string.IsNullOrEmpty(filtros.TipoLogro))
                {
                    query = query.Where(l => l.TipoLogro == filtros.TipoLogro);
                }

                if (filtros.SoloActivos == true)
                {
                    query = query.Where(l => l.EstaActivo);
                }

                if (filtros.PuntosMinimos.HasValue)
                {
                    query = query.Where(l => l.PuntosValor >= filtros.PuntosMinimos.Value);
                }

                if (filtros.PuntosMaximos.HasValue)
                {
                    query = query.Where(l => l.PuntosValor <= filtros.PuntosMaximos.Value);
                }
            }

            var logros = await query
                .OrderByDescending(l => l.CreadoEn)
                .ToListAsync();

            return logros.Select(MapearLogroADto).ToList();
        }

        public async Task<LogroObtenidoFacilitadorDto> OtorgarLogroAsync(OtorgarLogroDto dto, int coordinadorId)
        {
            // Verificar que el logro existe
            var logro = await _context.LogrosFacilitador.FindAsync(dto.IdLogro);
            if (logro == null || !logro.EstaActivo)
            {
                throw new ArgumentException($"Logro con ID {dto.IdLogro} no existe o está inactivo");
            }

            // Verificar que el facilitador existe y tiene el rol correcto (IdRol = 3)
            var facilitador = await _context.Usuarios.FindAsync(dto.IdFacilitador);
            if (facilitador == null || !facilitador.EstaActivo)
            {
                throw new ArgumentException($"Facilitador con ID {dto.IdFacilitador} no existe o está inactivo");
            }

            if (facilitador.IdRol != 3) // 3 = Facilitador
            {
                throw new ArgumentException($"El usuario con ID {dto.IdFacilitador} no es un facilitador");
            }

            // Verificar si ya tiene este logro
            var yaExiste = await _context.LogrosObtenidosFacilitador
                .AnyAsync(lo => lo.IdFacilitador == dto.IdFacilitador && lo.IdLogro == dto.IdLogro);

            if (yaExiste)
            {
                throw new InvalidOperationException($"El facilitador ya tiene este logro");
            }

            var logroObtenido = new LogroObtenidoFacilitador
            {
                IdLogro = dto.IdLogro,
                IdFacilitador = dto.IdFacilitador,
                FechaObtencion = DateTime.UtcNow,
                OtorgadoPorIdUsuario = coordinadorId,
                IdActividadRelacionada = dto.IdActividadRelacionada,
                IdActividadMontessoriRelacionada = dto.IdActividadMontessoriRelacionada,
                IdActividadPersonalizadaRelacionada = dto.IdActividadPersonalizadaRelacionada,
                Justificacion = dto.Justificacion,
                ComentarioCoordinador = dto.ComentarioCoordinador,
                EsVisible = true,
                CreadoEn = DateTime.UtcNow
            };

            _context.LogrosObtenidosFacilitador.Add(logroObtenido);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Logro {dto.IdLogro} otorgado al facilitador {dto.IdFacilitador} por coordinador {coordinadorId}");

            return await ObtenerLogroObtenidoPorIdAsync(logroObtenido.Id)
                ?? throw new Exception("Error al recuperar el logro otorgado");
        }

        public async Task<bool> RevocarLogroAsync(int idLogroObtenido, int coordinadorId)
        {
            var logroObtenido = await _context.LogrosObtenidosFacilitador.FindAsync(idLogroObtenido);
            if (logroObtenido == null)
            {
                return false;
            }

            _context.LogrosObtenidosFacilitador.Remove(logroObtenido);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Logro obtenido {idLogroObtenido} revocado por coordinador {coordinadorId}");

            return true;
        }

        public async Task<List<LogroObtenidoFacilitadorDto>> ObtenerLogrosObtenidosAsync(FiltrosLogrosObtenidosDto? filtros = null)
        {
            var query = _context.LogrosObtenidosFacilitador
                .Include(lo => lo.Logro)
                .Include(lo => lo.Facilitador)
                .Include(lo => lo.OtorgadoPor)
                .Include(lo => lo.ActividadRelacionada)
                .Include(lo => lo.ActividadMontessoriRelacionada)
                .Include(lo => lo.ActividadPersonalizadaRelacionada)
                .Where(lo => lo.EsVisible)
                .AsQueryable();

            if (filtros != null)
            {
                if (filtros.IdFacilitador.HasValue)
                {
                    query = query.Where(lo => lo.IdFacilitador == filtros.IdFacilitador.Value);
                }

                if (filtros.IdLogro.HasValue)
                {
                    query = query.Where(lo => lo.IdLogro == filtros.IdLogro.Value);
                }

                if (!string.IsNullOrEmpty(filtros.Categoria))
                {
                    query = query.Where(lo => lo.Logro.Categoria == filtros.Categoria);
                }

                if (filtros.FechaDesde.HasValue)
                {
                    query = query.Where(lo => lo.FechaObtencion >= filtros.FechaDesde.Value);
                }

                if (filtros.FechaHasta.HasValue)
                {
                    query = query.Where(lo => lo.FechaObtencion <= filtros.FechaHasta.Value);
                }

                if (filtros.OtorgadoPorIdUsuario.HasValue)
                {
                    query = query.Where(lo => lo.OtorgadoPorIdUsuario == filtros.OtorgadoPorIdUsuario.Value);
                }
            }

            var logrosObtenidos = await query
                .OrderByDescending(lo => lo.FechaObtencion)
                .ToListAsync();

            return logrosObtenidos.Select(MapearLogroObtenidoADto).ToList();
        }

        public async Task<PerfilLogrosFacilitadorDto> ObtenerPerfilFacilitadorAsync(int facilitadorId)
        {
            var facilitador = await _context.Usuarios.FindAsync(facilitadorId);
            if (facilitador == null)
            {
                throw new KeyNotFoundException($"Facilitador con ID {facilitadorId} no encontrado");
            }

            var logrosObtenidos = await _context.LogrosObtenidosFacilitador
                .Include(lo => lo.Logro)
                .Include(lo => lo.OtorgadoPor)
                .Include(lo => lo.ActividadRelacionada)
                .Include(lo => lo.ActividadMontessoriRelacionada)
                .Include(lo => lo.ActividadPersonalizadaRelacionada)
                .Where(lo => lo.IdFacilitador == facilitadorId && lo.EsVisible)
                .ToListAsync();

            var totalPuntos = logrosObtenidos.Sum(lo => lo.Logro.PuntosValor);

            // Calcular ranking
            var ranking = await ObtenerRankingFacilitadoresAsync(1000); // Obtener todos
            var posicion = ranking.FindIndex(r => r.IdFacilitador == facilitadorId) + 1;

            return new PerfilLogrosFacilitadorDto
            {
                IdFacilitador = facilitadorId,
                NombreFacilitador = $"{facilitador.PrimerNombre} {facilitador.Apellido}",
                EmailFacilitador = facilitador.CorreoElectronico,
                TotalLogrosObtenidos = logrosObtenidos.Count,
                TotalPuntosAcumulados = totalPuntos,
                LogrosObtenidos = logrosObtenidos.Select(MapearLogroObtenidoADto).ToList(),
                LogrosPorCategoria = logrosObtenidos
                    .Where(lo => !string.IsNullOrEmpty(lo.Logro.Categoria))
                    .GroupBy(lo => lo.Logro.Categoria!)
                    .ToDictionary(g => g.Key, g => g.Count()),
                PrimerLogroObtenido = logrosObtenidos.Any() 
                    ? logrosObtenidos.Min(lo => lo.FechaObtencion) 
                    : null,
                UltimoLogroObtenido = logrosObtenidos.Any() 
                    ? logrosObtenidos.Max(lo => lo.FechaObtencion) 
                    : null,
                RankingPosicion = posicion,
                TotalFacilitadores = ranking.Count
            };
        }

        public async Task<List<RankingFacilitadorDto>> ObtenerRankingFacilitadoresAsync(int top = 10)
        {
            var facilitadores = await _context.Usuarios
                .Where(u => u.IdRol == 3 && u.EstaActivo) // Solo facilitadores activos
                .Select(u => new
                {
                    u.Id,
                    u.PrimerNombre,
                    u.Apellido,
                    u.CorreoElectronico
                })
                .ToListAsync();

            var logrosObtenidos = await _context.LogrosObtenidosFacilitador
                .Include(lo => lo.Logro)
                .Where(lo => lo.EsVisible)
                .ToListAsync();

            var ranking = facilitadores.Select(f => new RankingFacilitadorDto
            {
                IdFacilitador = f.Id,
                NombreFacilitador = $"{f.PrimerNombre} {f.Apellido}",
                EmailFacilitador = f.CorreoElectronico,
                TotalLogros = logrosObtenidos.Count(lo => lo.IdFacilitador == f.Id),
                TotalPuntos = logrosObtenidos
                    .Where(lo => lo.IdFacilitador == f.Id)
                    .Sum(lo => lo.Logro.PuntosValor),
                UltimoLogroFecha = logrosObtenidos
                    .Where(lo => lo.IdFacilitador == f.Id)
                    .OrderByDescending(lo => lo.FechaObtencion)
                    .Select(lo => (DateTime?)lo.FechaObtencion)
                    .FirstOrDefault()
            })
            .OrderByDescending(r => r.TotalPuntos)
            .ThenByDescending(r => r.TotalLogros)
            .ToList();

            // Asignar posiciones
            for (int i = 0; i < ranking.Count; i++)
            {
                ranking[i].Posicion = i + 1;
            }

            return ranking.Take(top).ToList();
        }

        public async Task<EstadisticasLogrosDto> ObtenerEstadisticasGeneralesAsync()
        {
            var totalLogrosDefinidos = await _context.LogrosFacilitador.CountAsync();
            var totalLogrosActivos = await _context.LogrosFacilitador.CountAsync(l => l.EstaActivo);
            var totalLogrosOtorgados = await _context.LogrosObtenidosFacilitador.CountAsync();
            
            var facilitadoresConLogros = await _context.LogrosObtenidosFacilitador
                .Select(lo => lo.IdFacilitador)
                .Distinct()
                .CountAsync();

            var totalPuntosOtorgados = await _context.LogrosObtenidosFacilitador
                .Include(lo => lo.Logro)
                .SumAsync(lo => lo.Logro.PuntosValor);

            var logrosPorCategoria = await _context.LogrosObtenidosFacilitador
                .Include(lo => lo.Logro)
                .Where(lo => lo.Logro.Categoria != null)
                .GroupBy(lo => lo.Logro.Categoria!)
                .Select(g => new { Categoria = g.Key, Total = g.Count() })
                .ToDictionaryAsync(x => x.Categoria, x => x.Total);

            var logrosMasOtorgados = await _context.LogrosFacilitador
                .Include(l => l.CreadoPor)
                .Include(l => l.LogrosObtenidos)
                .Where(l => l.EstaActivo)
                .OrderByDescending(l => l.LogrosObtenidos.Count)
                .Take(5)
                .ToListAsync();

            var topFacilitadores = await ObtenerRankingFacilitadoresAsync(5);

            return new EstadisticasLogrosDto
            {
                TotalLogrosDefinidos = totalLogrosDefinidos,
                TotalLogrosActivos = totalLogrosActivos,
                TotalLogrosOtorgados = totalLogrosOtorgados,
                TotalFacilitadoresConLogros = facilitadoresConLogros,
                TotalPuntosOtorgados = totalPuntosOtorgados,
                LogrosOtorgadosPorCategoria = logrosPorCategoria,
                LogrosMasOtorgados = logrosMasOtorgados.Select(MapearLogroADto).ToList(),
                TopFacilitadores = topFacilitadores
            };
        }

        private async Task<LogroObtenidoFacilitadorDto?> ObtenerLogroObtenidoPorIdAsync(int id)
        {
            var logroObtenido = await _context.LogrosObtenidosFacilitador
                .Include(lo => lo.Logro)
                .Include(lo => lo.Facilitador)
                .Include(lo => lo.OtorgadoPor)
                .Include(lo => lo.ActividadRelacionada)
                .Include(lo => lo.ActividadMontessoriRelacionada)
                .Include(lo => lo.ActividadPersonalizadaRelacionada)
                .FirstOrDefaultAsync(lo => lo.Id == id);

            if (logroObtenido == null) return null;

            return MapearLogroObtenidoADto(logroObtenido);
        }

        private LogroFacilitadorDto MapearLogroADto(LogroFacilitador logro)
        {
            return new LogroFacilitadorDto
            {
                Id = logro.Id,
                Nombre = logro.Nombre,
                Descripcion = logro.Descripcion,
                Categoria = logro.Categoria,
                Icono = logro.Icono,
                PuntosValor = logro.PuntosValor,
                CriteriosObtencion = logro.CriteriosObtencion,
                TipoLogro = logro.TipoLogro,
                CantidadActividadesRequeridas = logro.CantidadActividadesRequeridas,
                CantidadEstudiantesRequeridos = logro.CantidadEstudiantesRequeridos,
                DiasConsecutivosRequeridos = logro.DiasConsecutivosRequeridos,
                CreadoPorIdUsuario = logro.CreadoPorIdUsuario,
                NombreCreadoPor = logro.CreadoPor != null 
                    ? $"{logro.CreadoPor.PrimerNombre} {logro.CreadoPor.Apellido}"
                    : "Desconocido",
                CreadoEn = logro.CreadoEn,
                ActualizadoEn = logro.ActualizadoEn,
                EstaActivo = logro.EstaActivo,
                TotalVecesOtorgado = logro.LogrosObtenidos.Count
            };
        }

        private LogroObtenidoFacilitadorDto MapearLogroObtenidoADto(LogroObtenidoFacilitador logroObtenido)
        {
            return new LogroObtenidoFacilitadorDto
            {
                Id = logroObtenido.Id,
                IdLogro = logroObtenido.IdLogro,
                NombreLogro = logroObtenido.Logro.Nombre,
                DescripcionLogro = logroObtenido.Logro.Descripcion,
                CategoriaLogro = logroObtenido.Logro.Categoria,
                IconoLogro = logroObtenido.Logro.Icono,
                PuntosLogro = logroObtenido.Logro.PuntosValor,
                IdFacilitador = logroObtenido.IdFacilitador,
                NombreFacilitador = $"{logroObtenido.Facilitador.PrimerNombre} {logroObtenido.Facilitador.Apellido}",
                FechaObtencion = logroObtenido.FechaObtencion,
                OtorgadoPorIdUsuario = logroObtenido.OtorgadoPorIdUsuario,
                NombreOtorgadoPor = logroObtenido.OtorgadoPor != null
                    ? $"{logroObtenido.OtorgadoPor.PrimerNombre} {logroObtenido.OtorgadoPor.Apellido}"
                    : "Desconocido",
                IdActividadRelacionada = logroObtenido.IdActividadRelacionada,
                NombreActividadRelacionada = logroObtenido.ActividadRelacionada?.Titulo,
                IdActividadMontessoriRelacionada = logroObtenido.IdActividadMontessoriRelacionada,
                NombreActividadMontessoriRelacionada = logroObtenido.ActividadMontessoriRelacionada?.Nombre,
                IdActividadPersonalizadaRelacionada = logroObtenido.IdActividadPersonalizadaRelacionada,
                NombreActividadPersonalizadaRelacionada = logroObtenido.ActividadPersonalizadaRelacionada?.Nombre,
                Justificacion = logroObtenido.Justificacion,
                ComentarioCoordinador = logroObtenido.ComentarioCoordinador,
                EsVisible = logroObtenido.EsVisible,
                CreadoEn = logroObtenido.CreadoEn
            };
        }
    }
}
