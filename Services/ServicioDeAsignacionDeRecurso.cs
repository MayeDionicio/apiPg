using ApiPG.DTOs;
using ApiPG.Data;
using ApiPG.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPG.Services
{
    public class ResourceAssignmentService : IResourceAssignmentService
    {
        private readonly ApiPGContext _db;
        private readonly IResourceService _resourceService;

        public ResourceAssignmentService(ApiPGContext db, IResourceService resourceService)
        {
            _db = db;
            _resourceService = resourceService;
        }

        public async Task<IEnumerable<AsignacionDeRecursoDto>> GetAllAsync()
        {
            var assignments = await _db.AsignacionesDeRecurso
                .Include(a => a.Recurso)
                .Include(a => a.Voluntario)
                .Include(a => a.AsignadoPor)
                .Include(a => a.RegistrosDeUso)
                .OrderByDescending(a => a.AsignadoEn)
                .ToListAsync();

            return assignments.Select(MapToDto);
        }

        public async Task<AsignacionDeRecursoDto?> GetByIdAsync(int id)
        {
            var assignment = await _db.AsignacionesDeRecurso
                .Include(a => a.Recurso)
                .Include(a => a.Voluntario)
                .Include(a => a.AsignadoPor)
                .Include(a => a.RegistrosDeUso)
                .FirstOrDefaultAsync(a => a.Id == id);

            return assignment != null ? MapToDto(assignment) : null;
        }

        public async Task<AsignacionDeRecursoDto> CreateAsync(CrearAsignacionDeRecursoDto dto, int assignedByUserId)
        {
            // Verificar que el recurso existe y tiene cantidad disponible
            var resource = await _db.Recursos.FindAsync(dto.RecursoId);
            if (resource == null)
                throw new ArgumentException("El recurso especificado no existe");

            if (!resource.EstaActivo)
                throw new ArgumentException("El recurso no está activo");

            if (resource.CantidadDisponible < dto.CantidadAsignada)
                throw new ArgumentException("No hay suficiente cantidad disponible del recurso");

            // Verificar que el voluntario existe
            var volunteer = await _db.Usuarios.FindAsync(dto.VoluntarioId);
            if (volunteer == null || !volunteer.EstaActivo)
                throw new ArgumentException("El voluntario especificado no existe o no está activo");

            // Reservar la cantidad del recurso
            await _resourceService.ReserveQuantityAsync(dto.RecursoId, dto.CantidadAsignada);

            var assignment = new AsignacionDeRecurso
            {
                IdRecurso = dto.RecursoId,
                IdVoluntario = dto.VoluntarioId,
                CantidadAsignada = dto.CantidadAsignada,
                Estado = EstadoDeAsignacion.Pendiente,
                AsignadoEn = DateTime.UtcNow,
                FechaEsperadaDeDevolucion = dto.FechaEsperadaDeDevolucion,
                NotasIniciales = dto.NotasIniciales,
                AsignadoPorIdUsuario = assignedByUserId,
                EstaActivo = true
            };

            _db.AsignacionesDeRecurso.Add(assignment);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(assignment.Id) ?? throw new InvalidOperationException("Error al crear la asignación");
        }

        public async Task<AsignacionDeRecursoDto?> UpdateAsync(int id, ActualizarAsignacionDeRecursoDto dto)
        {
            var assignment = await _db.AsignacionesDeRecurso.FindAsync(id);
            if (assignment == null) return null;

            if (dto.CantidadAsignada.HasValue && dto.CantidadAsignada.Value != assignment.CantidadAsignada)
            {
                var quantityDiff = dto.CantidadAsignada.Value - assignment.CantidadAsignada;
                
                if (quantityDiff > 0)
                {
                    // Necesita más cantidad, verificar disponibilidad
                    if (!await _resourceService.ReserveQuantityAsync(assignment.IdRecurso, quantityDiff))
                        throw new ArgumentException("No hay suficiente cantidad disponible");
                }
                else
                {
                    // Liberar cantidad sobrante
                    await _resourceService.ReleaseQuantityAsync(assignment.IdRecurso, Math.Abs(quantityDiff));
                }

                assignment.CantidadAsignada = dto.CantidadAsignada.Value;
            }

            if (dto.Estado.HasValue)
                assignment.Estado = dto.Estado.Value;

            if (dto.FechaEsperadaDeDevolucion.HasValue)
                assignment.FechaEsperadaDeDevolucion = dto.FechaEsperadaDeDevolucion.Value;

            if (dto.NotasIniciales != null)
                assignment.NotasIniciales = dto.NotasIniciales;

            if (dto.NotasDelVoluntario != null)
                assignment.NotasDelVoluntario = dto.NotasDelVoluntario;

            await _db.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var assignment = await _db.AsignacionesDeRecurso.FindAsync(id);
            if (assignment == null) return false;

            // Liberar la cantidad reservada si no se ha iniciado el uso
            if (assignment.Estado == EstadoDeAsignacion.Pendiente || assignment.Estado == EstadoDeAsignacion.Confirmado)
            {
                await _resourceService.ReleaseQuantityAsync(assignment.IdRecurso, assignment.CantidadAsignada);
            }

            assignment.EstaActivo = false;
            assignment.Estado = EstadoDeAsignacion.Cancelado;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<AsignacionDeRecursoDto?> ConfirmAssignmentAsync(int assignmentId, int volunteerId, ConfirmarAsignacionDto dto)
        {
            var assignment = await _db.AsignacionesDeRecurso.FindAsync(assignmentId);
            if (assignment == null) return null;

            if (assignment.IdVoluntario != volunteerId)
                throw new UnauthorizedAccessException("Solo el voluntario asignado puede confirmar la asignación");

            if (assignment.Estado != EstadoDeAsignacion.Pendiente)
                throw new InvalidOperationException("La asignación ya ha sido procesada");

            assignment.Estado = EstadoDeAsignacion.Confirmado;
            assignment.ConfirmadoEn = DateTime.UtcNow;
            assignment.NotasDelVoluntario = dto.NotasDelVoluntario;

            await _db.SaveChangesAsync();
            return await GetByIdAsync(assignmentId);
        }

        public async Task<AsignacionDeRecursoDto?> StartUseAsync(int assignmentId, int volunteerId)
        {
            var assignment = await _db.AsignacionesDeRecurso.FindAsync(assignmentId);
            if (assignment == null) return null;

            if (assignment.IdVoluntario != volunteerId)
                throw new UnauthorizedAccessException("Solo el voluntario asignado puede iniciar el uso");

            if (assignment.Estado != EstadoDeAsignacion.Confirmado)
                throw new InvalidOperationException("La asignación debe estar confirmada para iniciar el uso");

            assignment.Estado = EstadoDeAsignacion.EnUso;
            assignment.IniciadoEn = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return await GetByIdAsync(assignmentId);
        }

        public async Task<AsignacionDeRecursoDto?> ReturnResourceAsync(int assignmentId, int volunteerId)
        {
            var assignment = await _db.AsignacionesDeRecurso.FindAsync(assignmentId);
            if (assignment == null) return null;

            if (assignment.IdVoluntario != volunteerId)
                throw new UnauthorizedAccessException("Solo el voluntario asignado puede devolver el recurso");

            if (assignment.Estado != EstadoDeAsignacion.EnUso)
                throw new InvalidOperationException("El recurso debe estar en uso para poder devolverlo");

            assignment.Estado = EstadoDeAsignacion.Devuelto;
            assignment.DevueltoEn = DateTime.UtcNow;

            // Liberar la cantidad del recurso
            await _resourceService.ReleaseQuantityAsync(assignment.IdRecurso, assignment.CantidadAsignada);

            await _db.SaveChangesAsync();
            return await GetByIdAsync(assignmentId);
        }

        public async Task<AsignacionDeRecursoDto?> CancelAssignmentAsync(int assignmentId, int userId)
        {
            var assignment = await _db.AsignacionesDeRecurso.FindAsync(assignmentId);
            if (assignment == null) return null;

            if (assignment.Estado == EstadoDeAsignacion.EnUso)
                throw new InvalidOperationException("No se puede cancelar una asignación que está en uso");

            if (assignment.Estado == EstadoDeAsignacion.Devuelto)
                throw new InvalidOperationException("No se puede cancelar una asignación ya devuelta");

            // Liberar la cantidad reservada
            if (assignment.Estado == EstadoDeAsignacion.Pendiente || assignment.Estado == EstadoDeAsignacion.Confirmado)
            {
                await _resourceService.ReleaseQuantityAsync(assignment.IdRecurso, assignment.CantidadAsignada);
            }

            assignment.Estado = EstadoDeAsignacion.Cancelado;
            await _db.SaveChangesAsync();

            return await GetByIdAsync(assignmentId);
        }

        public async Task<IEnumerable<AsignacionDeRecursoDto>> GetByVolunteerAsync(int volunteerId)
        {
            var assignments = await _db.AsignacionesDeRecurso
                .Where(a => a.IdVoluntario == volunteerId && a.EstaActivo)
                .Include(a => a.Recurso)
                .Include(a => a.Voluntario)
                .Include(a => a.AsignadoPor)
                .Include(a => a.RegistrosDeUso)
                .OrderByDescending(a => a.AsignadoEn)
                .ToListAsync();

            return assignments.Select(MapToDto);
        }

        public async Task<IEnumerable<AsignacionDeRecursoDto>> GetByResourceAsync(int resourceId)
        {
            var assignments = await _db.AsignacionesDeRecurso
                .Where(a => a.IdRecurso == resourceId && a.EstaActivo)
                .Include(a => a.Recurso)
                .Include(a => a.Voluntario)
                .Include(a => a.AsignadoPor)
                .Include(a => a.RegistrosDeUso)
                .OrderByDescending(a => a.AsignadoEn)
                .ToListAsync();

            return assignments.Select(MapToDto);
        }

        public async Task<IEnumerable<AsignacionDeRecursoDto>> GetPendingAssignmentsAsync()
        {
            var assignments = await _db.AsignacionesDeRecurso
                .Where(a => a.Estado == EstadoDeAsignacion.Pendiente && a.EstaActivo)
                .Include(a => a.Recurso)
                .Include(a => a.Voluntario)
                .Include(a => a.AsignadoPor)
                .Include(a => a.RegistrosDeUso)
                .OrderBy(a => a.AsignadoEn)
                .ToListAsync();

            return assignments.Select(MapToDto);
        }

        public async Task<IEnumerable<AsignacionDeRecursoDto>> GetOverdueReturnsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var assignments = await _db.AsignacionesDeRecurso
                .Where(a => a.Estado == EstadoDeAsignacion.EnUso && 
                           a.EstaActivo && 
                           a.FechaEsperadaDeDevolucion.HasValue && 
                           a.FechaEsperadaDeDevolucion.Value.Date < today)
                .Include(a => a.Recurso)
                .Include(a => a.Voluntario)
                .Include(a => a.AsignadoPor)
                .Include(a => a.RegistrosDeUso)
                .OrderBy(a => a.FechaEsperadaDeDevolucion)
                .ToListAsync();

            return assignments.Select(MapToDto);
        }

        public async Task<IEnumerable<AsignacionDeRecursoDto>> GetActiveAssignmentsAsync()
        {
            var assignments = await _db.AsignacionesDeRecurso
                .Where(a => (a.Estado == EstadoDeAsignacion.Pendiente || 
                            a.Estado == EstadoDeAsignacion.Confirmado || 
                            a.Estado == EstadoDeAsignacion.EnUso) && a.EstaActivo)
                .Include(a => a.Recurso)
                .Include(a => a.Voluntario)
                .Include(a => a.AsignadoPor)
                .Include(a => a.RegistrosDeUso)
                .OrderByDescending(a => a.AsignadoEn)
                .ToListAsync();

            return assignments.Select(MapToDto);
        }

        public async Task<object> GetAssignmentStatisticsAsync()
        {
            var totalAssignments = await _db.AsignacionesDeRecurso.CountAsync(a => a.EstaActivo);
            var pendingCount = await _db.AsignacionesDeRecurso.CountAsync(a => a.Estado == EstadoDeAsignacion.Pendiente && a.EstaActivo);
            var confirmedCount = await _db.AsignacionesDeRecurso.CountAsync(a => a.Estado == EstadoDeAsignacion.Confirmado && a.EstaActivo);
            var inUseCount = await _db.AsignacionesDeRecurso.CountAsync(a => a.Estado == EstadoDeAsignacion.EnUso && a.EstaActivo);
            var returnedCount = await _db.AsignacionesDeRecurso.CountAsync(a => a.Estado == EstadoDeAsignacion.Devuelto && a.EstaActivo);
            var cancelledCount = await _db.AsignacionesDeRecurso.CountAsync(a => a.Estado == EstadoDeAsignacion.Cancelado && a.EstaActivo);

            var today = DateTime.UtcNow.Date;
            var overdueCount = await _db.AsignacionesDeRecurso.CountAsync(a => 
                a.Estado == EstadoDeAsignacion.EnUso && 
                a.EstaActivo && 
                a.FechaEsperadaDeDevolucion.HasValue && 
                a.FechaEsperadaDeDevolucion.Value.Date < today);

            return new
            {
                TotalAssignments = totalAssignments,
                PendingCount = pendingCount,
                ConfirmedCount = confirmedCount,
                InUseCount = inUseCount,
                ReturnedCount = returnedCount,
                CancelledCount = cancelledCount,
                OverdueCount = overdueCount
            };
        }

        private static AsignacionDeRecursoDto MapToDto(AsignacionDeRecurso assignment)
        {
            return new AsignacionDeRecursoDto
            {
                Id = assignment.Id,
                RecursoId = assignment.IdRecurso,
                NombreRecurso = assignment.Recurso.Nombre,
                CategoriaRecurso = assignment.Recurso.Categoria,
                VoluntarioId = assignment.IdVoluntario,
                NombreVoluntario = $"{assignment.Voluntario.PrimerNombre} {assignment.Voluntario.Apellido}",
                CantidadAsignada = assignment.CantidadAsignada,
                Estado = assignment.Estado,
                AsignadoEn = assignment.AsignadoEn,
                ConfirmadoEn = assignment.ConfirmadoEn,
                IniciadoEn = assignment.IniciadoEn,
                DevueltoEn = assignment.DevueltoEn,
                FechaEsperadaDeDevolucion = assignment.FechaEsperadaDeDevolucion,
                NotasIniciales = assignment.NotasIniciales,
                NotasDelVoluntario = assignment.NotasDelVoluntario,
                AsignadoPorIdUsuario = assignment.AsignadoPorIdUsuario,
                AsignadoPorNombreUsuario = assignment.AsignadoPor != null ? $"{assignment.AsignadoPor.PrimerNombre} {assignment.AsignadoPor.Apellido}" : null,
                EstaActivo = assignment.EstaActivo,
                CantidadRegistrosDeUso = assignment.RegistrosDeUso.Count,
                TieneIncidentes = assignment.RegistrosDeUso.Any(l => l.TipoDeEvento == TipoDeEventoDeUso.ReporteDeIncidente || l.TipoDeEvento == TipoDeEventoDeUso.ReporteDeDanio)
            };
        }
    }
}
