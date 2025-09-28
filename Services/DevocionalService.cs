using Microsoft.EntityFrameworkCore;
using ApiPG.Data;
using ApiPG.DTOs;
using ApiPG.Models;
using System.Text.Json;

namespace ApiPG.Services
{
    public class DevocionalService : IDevocionalService
    {
        private readonly ApiPGContext _context;

        public DevocionalService(ApiPGContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DevocionalDto>> GetAllAsync()
        {
            var devocionales = await _context.Devocionales
                .Include(d => d.CreatedByUser)
                .Include(d => d.VoluntarioAsignadoUser)
                .Where(d => d.IsActive)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return devocionales.Select(MapToDto);
        }

        public async Task<DevocionalDto?> GetByIdAsync(int id)
        {
            var devocional = await _context.Devocionales
                .Include(d => d.CreatedByUser)
                .Include(d => d.VoluntarioAsignadoUser)
                .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);

            return devocional != null ? MapToDto(devocional) : null;
        }

        public async Task<DevocionalDto> CreateAsync(CreateDevocionalDto createDto, int createdByUserId)
        {
            var devocional = new Devocional
            {
                Titulo = createDto.Titulo,
                FechaProgramada = createDto.FechaProgramada,
                VoluntarioAsignadoId = createDto.VoluntarioAsignadoId,
                VoluntarioAsignado = createDto.VoluntarioAsignado,
                Pasaje = createDto.Pasaje,
                TextoClave = createDto.TextoClave,
                Objetivo = createDto.Objetivo,
                Idea = createDto.Idea,
                PuntosPrincipales = createDto.PuntosPrincipales != null ? 
                    JsonSerializer.Serialize(createDto.PuntosPrincipales) : null,
                Aplicacion = createDto.Aplicacion,
                Reto = createDto.Reto,
                Oracion = createDto.Oracion,
                Recursos = createDto.Recursos,
                Estado = createDto.Estado,
                CreatedByUserId = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Devocionales.Add(devocional);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(devocional.Id) ?? throw new InvalidOperationException("Error al crear el devocional");
        }

        public async Task<DevocionalDto?> UpdateAsync(int id, UpdateDevocionalDto updateDto, int updatedByUserId)
        {
            var devocional = await _context.Devocionales.FindAsync(id);
            if (devocional == null || !devocional.IsActive) return null;

            // Solo actualizar campos que no son null
            if (updateDto.Titulo != null) devocional.Titulo = updateDto.Titulo;
            if (updateDto.FechaProgramada.HasValue) devocional.FechaProgramada = updateDto.FechaProgramada;
            if (updateDto.VoluntarioAsignadoId.HasValue) devocional.VoluntarioAsignadoId = updateDto.VoluntarioAsignadoId;
            if (updateDto.VoluntarioAsignado != null) devocional.VoluntarioAsignado = updateDto.VoluntarioAsignado;
            if (updateDto.Pasaje != null) devocional.Pasaje = updateDto.Pasaje;
            if (updateDto.TextoClave != null) devocional.TextoClave = updateDto.TextoClave;
            if (updateDto.Objetivo != null) devocional.Objetivo = updateDto.Objetivo;
            if (updateDto.Idea != null) devocional.Idea = updateDto.Idea;
            if (updateDto.PuntosPrincipales != null) 
                devocional.PuntosPrincipales = JsonSerializer.Serialize(updateDto.PuntosPrincipales);
            if (updateDto.Aplicacion != null) devocional.Aplicacion = updateDto.Aplicacion;
            if (updateDto.Reto != null) devocional.Reto = updateDto.Reto;
            if (updateDto.Oracion != null) devocional.Oracion = updateDto.Oracion;
            if (updateDto.Recursos != null) devocional.Recursos = updateDto.Recursos;
            if (updateDto.Estado.HasValue) devocional.Estado = updateDto.Estado.Value;
            if (updateDto.IsActive.HasValue) devocional.IsActive = updateDto.IsActive.Value;

            devocional.UpdatedAt = DateTime.UtcNow;
            devocional.UpdatedByUserId = updatedByUserId;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var devocional = await _context.Devocionales.FindAsync(id);
            if (devocional == null) return false;

            devocional.IsActive = false;
            devocional.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateAsync(int id)
        {
            var devocional = await _context.Devocionales.FindAsync(id);
            if (devocional == null) return false;

            devocional.IsActive = true;
            devocional.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // Métodos específicos del wizard
        public async Task<DevocionalDto> SaveStep1Async(DevocionalStep1Dto step1, int userId)
        {
            try 
            {
                // Validar que el usuario existe
                if (userId <= 0)
                {
                    throw new ArgumentException("Usuario no válido");
                }

                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                if (!userExists)
                {
                    throw new ArgumentException($"Usuario con ID {userId} no existe");
                }

                // Validar voluntario si se proporciona
                if (step1.VoluntarioAsignadoId.HasValue && step1.VoluntarioAsignadoId.Value > 0)
                {
                    var voluntarioExists = await _context.Users.AnyAsync(u => u.Id == step1.VoluntarioAsignadoId.Value);
                    if (!voluntarioExists)
                    {
                        throw new ArgumentException($"Voluntario con ID {step1.VoluntarioAsignadoId.Value} no existe");
                    }
                }

                var devocional = new Devocional
                {
                    Titulo = step1.Titulo,
                    FechaProgramada = step1.FechaProgramada,
                    VoluntarioAsignado = step1.VoluntarioAsignado,
                    VoluntarioAsignadoId = step1.VoluntarioAsignadoId,
                    Pasaje = null, // Se llenará en step 2
                    Estado = DevocionalEstado.Borrador,
                    CreatedByUserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Devocionales.Add(devocional);
                
                try 
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception saveEx)
                {
                    throw new InvalidOperationException($"Error al guardar en base de datos: {saveEx.Message}", saveEx);
                }

                return await GetByIdAsync(devocional.Id) ?? throw new InvalidOperationException("Error al recuperar devocional guardado");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error en SaveStep1Async: {ex.Message}", ex);
            }
        }

        public async Task<DevocionalDto> SaveStep2Async(int devocionalId, DevocionalStep2Dto step2, int userId)
        {
            var devocional = await _context.Devocionales.FindAsync(devocionalId);
            if (devocional == null) throw new ArgumentException("Devocional no encontrado");

            devocional.Pasaje = step2.Pasaje;
            devocional.TextoClave = step2.TextoClave;
            devocional.UpdatedAt = DateTime.UtcNow;
            devocional.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(devocionalId) ?? throw new InvalidOperationException("Error al guardar paso 2");
        }

        public async Task<DevocionalDto> SaveStep3Async(int devocionalId, DevocionalStep3Dto step3, int userId)
        {
            var devocional = await _context.Devocionales.FindAsync(devocionalId);
            if (devocional == null) throw new ArgumentException("Devocional no encontrado");

            devocional.Objetivo = step3.Objetivo;
            devocional.Idea = step3.Idea;
            devocional.PuntosPrincipales = step3.PuntosPrincipales != null ? 
                JsonSerializer.Serialize(step3.PuntosPrincipales) : null;
            devocional.Aplicacion = step3.Aplicacion;
            devocional.Reto = step3.Reto;
            devocional.Oracion = step3.Oracion;
            devocional.Recursos = step3.Recursos;
            devocional.UpdatedAt = DateTime.UtcNow;
            devocional.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(devocionalId) ?? throw new InvalidOperationException("Error al guardar paso 3");
        }

        public async Task<DevocionalDto> SaveDraftAsync(SaveDraftDto draftDto, int userId)
        {
            Devocional devocional;

            if (draftDto.DevocionalId.HasValue)
            {
                // Actualizar borrador existente
                var existingDevocional = await _context.Devocionales.FindAsync(draftDto.DevocionalId.Value);
                if (existingDevocional == null) throw new ArgumentException("Devocional no encontrado");
                
                devocional = existingDevocional;
                devocional.UpdatedAt = DateTime.UtcNow;
                devocional.UpdatedByUserId = userId;
            }
            else
            {
                // Crear nuevo borrador
                devocional = new Devocional
                {
                    Estado = DevocionalEstado.Borrador,
                    CreatedByUserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    Pasaje = null // Se establecerá cuando se proporcione
                };
                _context.Devocionales.Add(devocional);
            }

            // Actualizar campos
            if (draftDto.Titulo != null) devocional.Titulo = draftDto.Titulo;
            if (draftDto.FechaProgramada.HasValue) devocional.FechaProgramada = draftDto.FechaProgramada;
            if (draftDto.VoluntarioAsignado != null) devocional.VoluntarioAsignado = draftDto.VoluntarioAsignado;
            if (draftDto.VoluntarioAsignadoId.HasValue) devocional.VoluntarioAsignadoId = draftDto.VoluntarioAsignadoId;
            if (draftDto.Pasaje != null) devocional.Pasaje = draftDto.Pasaje;
            if (draftDto.TextoClave != null) devocional.TextoClave = draftDto.TextoClave;
            if (draftDto.Objetivo != null) devocional.Objetivo = draftDto.Objetivo;
            if (draftDto.Idea != null) devocional.Idea = draftDto.Idea;
            if (draftDto.PuntosPrincipales != null) 
                devocional.PuntosPrincipales = JsonSerializer.Serialize(draftDto.PuntosPrincipales);
            if (draftDto.Aplicacion != null) devocional.Aplicacion = draftDto.Aplicacion;
            if (draftDto.Reto != null) devocional.Reto = draftDto.Reto;
            if (draftDto.Oracion != null) devocional.Oracion = draftDto.Oracion;
            if (draftDto.Recursos != null) devocional.Recursos = draftDto.Recursos;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(devocional.Id) ?? throw new InvalidOperationException("Error al guardar borrador");
        }

        public async Task<DevocionalDto> FinalizeDevocionalAsync(int devocionalId, int userId)
        {
            var devocional = await _context.Devocionales.FindAsync(devocionalId);
            if (devocional == null) throw new ArgumentException("Devocional no encontrado");

            // Validar que tenga los campos mínimos
            if (string.IsNullOrWhiteSpace(devocional.Titulo) || string.IsNullOrWhiteSpace(devocional.Pasaje))
                throw new InvalidOperationException("El devocional debe tener título y pasaje para ser finalizado");

            devocional.Estado = DevocionalEstado.Programado;
            devocional.UpdatedAt = DateTime.UtcNow;
            devocional.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(devocionalId) ?? throw new InvalidOperationException("Error al finalizar devocional");
        }

        // Filtros y consultas
        public async Task<IEnumerable<DevocionalDto>> GetByEstadoAsync(DevocionalEstado estado)
        {
            var devocionales = await _context.Devocionales
                .Include(d => d.CreatedByUser)
                .Include(d => d.VoluntarioAsignadoUser)
                .Where(d => d.IsActive && d.Estado == estado)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return devocionales.Select(MapToDto);
        }

        public async Task<IEnumerable<DevocionalDto>> GetByVoluntarioAsync(int voluntarioId)
        {
            var devocionales = await _context.Devocionales
                .Include(d => d.CreatedByUser)
                .Include(d => d.VoluntarioAsignadoUser)
                .Where(d => d.IsActive && d.VoluntarioAsignadoId == voluntarioId)
                .OrderByDescending(d => d.FechaProgramada ?? d.CreatedAt)
                .ToListAsync();

            return devocionales.Select(MapToDto);
        }

        public async Task<IEnumerable<DevocionalDto>> GetByFechaAsync(DateTime fecha)
        {
            var devocionales = await _context.Devocionales
                .Include(d => d.CreatedByUser)
                .Include(d => d.VoluntarioAsignadoUser)
                .Where(d => d.IsActive && d.FechaProgramada.HasValue && 
                           d.FechaProgramada.Value.Date == fecha.Date)
                .OrderBy(d => d.FechaProgramada)
                .ToListAsync();

            return devocionales.Select(MapToDto);
        }

        public async Task<IEnumerable<DevocionalDto>> GetByRangoFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var devocionales = await _context.Devocionales
                .Include(d => d.CreatedByUser)
                .Include(d => d.VoluntarioAsignadoUser)
                .Where(d => d.IsActive && d.FechaProgramada.HasValue &&
                           d.FechaProgramada.Value.Date >= fechaInicio.Date &&
                           d.FechaProgramada.Value.Date <= fechaFin.Date)
                .OrderBy(d => d.FechaProgramada)
                .ToListAsync();

            return devocionales.Select(MapToDto);
        }

        public async Task<IEnumerable<DevocionalDto>> GetBorradoresAsync()
        {
            return await GetByEstadoAsync(DevocionalEstado.Borrador);
        }

        public async Task<IEnumerable<DevocionalDto>> GetProgramadosAsync()
        {
            return await GetByEstadoAsync(DevocionalEstado.Programado);
        }

        public async Task<IEnumerable<DevocionalDto>> GetProximosAsync(int dias = 7)
        {
            var fechaLimite = DateTime.Today.AddDays(dias);
            return await GetByRangoFechaAsync(DateTime.Today, fechaLimite);
        }

        public async Task<IEnumerable<DevocionalDto>> GetRecentesAsync(int take = 10)
        {
            var devocionales = await _context.Devocionales
                .Include(d => d.CreatedByUser)
                .Include(d => d.VoluntarioAsignadoUser)
                .Where(d => d.IsActive)
                .OrderByDescending(d => d.UpdatedAt ?? d.CreatedAt)
                .Take(take)
                .ToListAsync();

            return devocionales.Select(MapToDto);
        }

        // Cambios de estado
        public async Task<DevocionalDto?> CambiarEstadoAsync(int id, DevocionalEstado nuevoEstado, int userId)
        {
            var devocional = await _context.Devocionales.FindAsync(id);
            if (devocional == null || !devocional.IsActive) return null;

            devocional.Estado = nuevoEstado;
            devocional.UpdatedAt = DateTime.UtcNow;
            devocional.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<DevocionalDto?> ProgrimarDevocionalAsync(int id, int userId)
        {
            return await CambiarEstadoAsync(id, DevocionalEstado.Programado, userId);
        }

        public async Task<DevocionalDto?> IniciarDevocionalAsync(int id, int userId)
        {
            return await CambiarEstadoAsync(id, DevocionalEstado.EnProgreso, userId);
        }

        public async Task<DevocionalDto?> CompletarDevocionalAsync(int id, int userId)
        {
            return await CambiarEstadoAsync(id, DevocionalEstado.Completado, userId);
        }

        public async Task<DevocionalDto?> CancelarDevocionalAsync(int id, int userId)
        {
            return await CambiarEstadoAsync(id, DevocionalEstado.Cancelado, userId);
        }

        // Utilidades
        public async Task<DevocionalPreviewDto?> GetPreviewAsync(int id)
        {
            var devocional = await _context.Devocionales.FindAsync(id);
            if (devocional == null || !devocional.IsActive) return null;

            var puntosPrincipales = !string.IsNullOrEmpty(devocional.PuntosPrincipales) ?
                JsonSerializer.Deserialize<List<PuntoPrincipal>>(devocional.PuntosPrincipales) : null;

            var contentParts = new List<string>();
            if (!string.IsNullOrEmpty(devocional.Objetivo)) contentParts.Add("objetivo");
            if (!string.IsNullOrEmpty(devocional.Idea)) contentParts.Add("idea central");
            if (puntosPrincipales?.Any() == true) contentParts.Add($"{puntosPrincipales.Count} puntos principales");
            if (!string.IsNullOrEmpty(devocional.Aplicacion)) contentParts.Add("aplicación práctica");
            if (!string.IsNullOrEmpty(devocional.Reto)) contentParts.Add("reto semanal");
            if (!string.IsNullOrEmpty(devocional.Oracion)) contentParts.Add("oración");
            if (!string.IsNullOrEmpty(devocional.Recursos)) contentParts.Add("recursos adicionales");

            return new DevocionalPreviewDto
            {
                Titulo = devocional.Titulo,
                FechaProgramada = devocional.FechaProgramada,
                VoluntarioAsignado = devocional.VoluntarioAsignado,
                Pasaje = devocional.Pasaje,
                Objetivo = devocional.Objetivo,
                Idea = devocional.Idea,
                ContentSummary = string.Join(", ", contentParts),
                HasContent = contentParts.Any()
            };
        }

        public async Task<IEnumerable<string>> GetVoluntariosDisponiblesAsync()
        {
            var voluntarios = await _context.Users
                .Where(u => u.IsActive && u.Role.Name == "Voluntario")
                .Select(u => $"{u.FirstName} {u.LastName}")
                .ToListAsync();

            return voluntarios;
        }

        public async Task<DevocionalEstadisticasDto> GetEstadisticasAsync()
        {
            var devocionales = await _context.Devocionales
                .Where(d => d.IsActive)
                .ToListAsync();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var nextWeek = today.AddDays(7);

            var stats = new DevocionalEstadisticasDto
            {
                TotalDevocionales = devocionales.Count,
                BorradoresCount = devocionales.Count(d => d.Estado == DevocionalEstado.Borrador),
                ProgramadosCount = devocionales.Count(d => d.Estado == DevocionalEstado.Programado),
                CompletadosCount = devocionales.Count(d => d.Estado == DevocionalEstado.Completado),
                CanceladosCount = devocionales.Count(d => d.Estado == DevocionalEstado.Cancelado),
                DevocionalesTodayCount = devocionales.Count(d => d.FechaProgramada?.Date == today),
                DevocionalesTomorrow = devocionales.Count(d => d.FechaProgramada?.Date == tomorrow),
                DevocionalessemanaCount = devocionales.Count(d => d.FechaProgramada?.Date >= today && 
                                                                  d.FechaProgramada?.Date <= nextWeek)
            };

            return stats;
        }

        public async Task<IEnumerable<DevocionalDto>> SearchAsync(string searchTerm)
        {
            var devocionales = await _context.Devocionales
                .Include(d => d.CreatedByUser)
                .Include(d => d.VoluntarioAsignadoUser)
                .Where(d => d.IsActive && 
                           (d.Titulo.Contains(searchTerm) ||
                            (d.Pasaje != null && d.Pasaje.Contains(searchTerm)) ||
                            (d.Objetivo != null && d.Objetivo.Contains(searchTerm)) ||
                            (d.Idea != null && d.Idea.Contains(searchTerm))))
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return devocionales.Select(MapToDto);
        }

        public async Task<IEnumerable<DevocionalDto>> GetByCreatedByUserAsync(int userId)
        {
            var devocionales = await _context.Devocionales
                .Include(d => d.CreatedByUser)
                .Include(d => d.VoluntarioAsignadoUser)
                .Where(d => d.IsActive && d.CreatedByUserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return devocionales.Select(MapToDto);
        }

        // Método helper para mapear entidad a DTO
        private DevocionalDto MapToDto(Devocional devocional)
        {
            List<PuntoPrincipal>? puntosPrincipales = null;
            
            if (!string.IsNullOrEmpty(devocional.PuntosPrincipales))
            {
                try
                {
                    puntosPrincipales = JsonSerializer.Deserialize<List<PuntoPrincipal>>(devocional.PuntosPrincipales);
                }
                catch
                {
                    puntosPrincipales = null;
                }
            }

            return new DevocionalDto
            {
                Id = devocional.Id,
                Titulo = devocional.Titulo,
                FechaProgramada = devocional.FechaProgramada,
                VoluntarioAsignadoId = devocional.VoluntarioAsignadoId,
                VoluntarioAsignado = devocional.VoluntarioAsignado,
                Pasaje = devocional.Pasaje ?? string.Empty,
                TextoClave = devocional.TextoClave,
                Objetivo = devocional.Objetivo,
                Idea = devocional.Idea,
                PuntosPrincipales = puntosPrincipales,
                Aplicacion = devocional.Aplicacion,
                Reto = devocional.Reto,
                Oracion = devocional.Oracion,
                Recursos = devocional.Recursos,
                Estado = devocional.Estado,
                CreatedAt = devocional.CreatedAt,
                UpdatedAt = devocional.UpdatedAt,
                CreatedByUserId = devocional.CreatedByUserId,
                CreatedByUserName = devocional.CreatedByUser?.FirstName + " " + devocional.CreatedByUser?.LastName,
                IsActive = devocional.IsActive
            };
        }
    }
}