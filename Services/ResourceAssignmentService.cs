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

        public async Task<IEnumerable<ResourceAssignmentDto>> GetAllAsync()
        {
            var assignments = await _db.ResourceAssignments
                .Include(a => a.Resource)
                .Include(a => a.Volunteer)
                .Include(a => a.AssignedByUser)
                .Include(a => a.UsageLogs)
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();

            return assignments.Select(MapToDto);
        }

        public async Task<ResourceAssignmentDto?> GetByIdAsync(int id)
        {
            var assignment = await _db.ResourceAssignments
                .Include(a => a.Resource)
                .Include(a => a.Volunteer)
                .Include(a => a.AssignedByUser)
                .Include(a => a.UsageLogs)
                .FirstOrDefaultAsync(a => a.Id == id);

            return assignment != null ? MapToDto(assignment) : null;
        }

        public async Task<ResourceAssignmentDto> CreateAsync(CreateResourceAssignmentDto dto, int assignedByUserId)
        {
            // Verificar que el recurso existe y tiene cantidad disponible
            var resource = await _db.Resources.FindAsync(dto.ResourceId);
            if (resource == null)
                throw new ArgumentException("El recurso especificado no existe");

            if (!resource.IsActive)
                throw new ArgumentException("El recurso no está activo");

            if (resource.AvailableQuantity < dto.QuantityAssigned)
                throw new ArgumentException("No hay suficiente cantidad disponible del recurso");

            // Verificar que el voluntario existe
            var volunteer = await _db.Users.FindAsync(dto.VolunteerId);
            if (volunteer == null || !volunteer.IsActive)
                throw new ArgumentException("El voluntario especificado no existe o no está activo");

            // Reservar la cantidad del recurso
            await _resourceService.ReserveQuantityAsync(dto.ResourceId, dto.QuantityAssigned);

            var assignment = new ResourceAssignment
            {
                ResourceId = dto.ResourceId,
                VolunteerId = dto.VolunteerId,
                QuantityAssigned = dto.QuantityAssigned,
                Status = AssignmentStatus.Pending,
                AssignedAt = DateTime.UtcNow,
                ExpectedReturnDate = dto.ExpectedReturnDate,
                InitialNotes = dto.InitialNotes,
                AssignedByUserId = assignedByUserId,
                IsActive = true
            };

            _db.ResourceAssignments.Add(assignment);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(assignment.Id) ?? throw new InvalidOperationException("Error al crear la asignación");
        }

        public async Task<ResourceAssignmentDto?> UpdateAsync(int id, UpdateResourceAssignmentDto dto)
        {
            var assignment = await _db.ResourceAssignments.FindAsync(id);
            if (assignment == null) return null;

            if (dto.QuantityAssigned.HasValue && dto.QuantityAssigned.Value != assignment.QuantityAssigned)
            {
                var quantityDiff = dto.QuantityAssigned.Value - assignment.QuantityAssigned;
                
                if (quantityDiff > 0)
                {
                    // Necesita más cantidad, verificar disponibilidad
                    if (!await _resourceService.ReserveQuantityAsync(assignment.ResourceId, quantityDiff))
                        throw new ArgumentException("No hay suficiente cantidad disponible");
                }
                else
                {
                    // Liberar cantidad sobrante
                    await _resourceService.ReleaseQuantityAsync(assignment.ResourceId, Math.Abs(quantityDiff));
                }

                assignment.QuantityAssigned = dto.QuantityAssigned.Value;
            }

            if (dto.Status.HasValue)
                assignment.Status = dto.Status.Value;

            if (dto.ExpectedReturnDate.HasValue)
                assignment.ExpectedReturnDate = dto.ExpectedReturnDate.Value;

            if (dto.InitialNotes != null)
                assignment.InitialNotes = dto.InitialNotes;

            if (dto.VolunteerNotes != null)
                assignment.VolunteerNotes = dto.VolunteerNotes;

            await _db.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var assignment = await _db.ResourceAssignments.FindAsync(id);
            if (assignment == null) return false;

            // Liberar la cantidad reservada si no se ha iniciado el uso
            if (assignment.Status == AssignmentStatus.Pending || assignment.Status == AssignmentStatus.Confirmed)
            {
                await _resourceService.ReleaseQuantityAsync(assignment.ResourceId, assignment.QuantityAssigned);
            }

            assignment.IsActive = false;
            assignment.Status = AssignmentStatus.Cancelled;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<ResourceAssignmentDto?> ConfirmAssignmentAsync(int assignmentId, int volunteerId, ConfirmAssignmentDto dto)
        {
            var assignment = await _db.ResourceAssignments.FindAsync(assignmentId);
            if (assignment == null) return null;

            if (assignment.VolunteerId != volunteerId)
                throw new UnauthorizedAccessException("Solo el voluntario asignado puede confirmar la asignación");

            if (assignment.Status != AssignmentStatus.Pending)
                throw new InvalidOperationException("La asignación ya ha sido procesada");

            assignment.Status = AssignmentStatus.Confirmed;
            assignment.ConfirmedAt = DateTime.UtcNow;
            assignment.VolunteerNotes = dto.VolunteerNotes;

            await _db.SaveChangesAsync();
            return await GetByIdAsync(assignmentId);
        }

        public async Task<ResourceAssignmentDto?> StartUseAsync(int assignmentId, int volunteerId)
        {
            var assignment = await _db.ResourceAssignments.FindAsync(assignmentId);
            if (assignment == null) return null;

            if (assignment.VolunteerId != volunteerId)
                throw new UnauthorizedAccessException("Solo el voluntario asignado puede iniciar el uso");

            if (assignment.Status != AssignmentStatus.Confirmed)
                throw new InvalidOperationException("La asignación debe estar confirmada para iniciar el uso");

            assignment.Status = AssignmentStatus.InUse;
            assignment.StartedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return await GetByIdAsync(assignmentId);
        }

        public async Task<ResourceAssignmentDto?> ReturnResourceAsync(int assignmentId, int volunteerId)
        {
            var assignment = await _db.ResourceAssignments.FindAsync(assignmentId);
            if (assignment == null) return null;

            if (assignment.VolunteerId != volunteerId)
                throw new UnauthorizedAccessException("Solo el voluntario asignado puede devolver el recurso");

            if (assignment.Status != AssignmentStatus.InUse)
                throw new InvalidOperationException("El recurso debe estar en uso para poder devolverlo");

            assignment.Status = AssignmentStatus.Returned;
            assignment.ReturnedAt = DateTime.UtcNow;

            // Liberar la cantidad del recurso
            await _resourceService.ReleaseQuantityAsync(assignment.ResourceId, assignment.QuantityAssigned);

            await _db.SaveChangesAsync();
            return await GetByIdAsync(assignmentId);
        }

        public async Task<ResourceAssignmentDto?> CancelAssignmentAsync(int assignmentId, int userId)
        {
            var assignment = await _db.ResourceAssignments.FindAsync(assignmentId);
            if (assignment == null) return null;

            if (assignment.Status == AssignmentStatus.InUse)
                throw new InvalidOperationException("No se puede cancelar una asignación que está en uso");

            if (assignment.Status == AssignmentStatus.Returned)
                throw new InvalidOperationException("No se puede cancelar una asignación ya devuelta");

            // Liberar la cantidad reservada
            if (assignment.Status == AssignmentStatus.Pending || assignment.Status == AssignmentStatus.Confirmed)
            {
                await _resourceService.ReleaseQuantityAsync(assignment.ResourceId, assignment.QuantityAssigned);
            }

            assignment.Status = AssignmentStatus.Cancelled;
            await _db.SaveChangesAsync();

            return await GetByIdAsync(assignmentId);
        }

        public async Task<IEnumerable<ResourceAssignmentDto>> GetByVolunteerAsync(int volunteerId)
        {
            var assignments = await _db.ResourceAssignments
                .Where(a => a.VolunteerId == volunteerId && a.IsActive)
                .Include(a => a.Resource)
                .Include(a => a.Volunteer)
                .Include(a => a.AssignedByUser)
                .Include(a => a.UsageLogs)
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();

            return assignments.Select(MapToDto);
        }

        public async Task<IEnumerable<ResourceAssignmentDto>> GetByResourceAsync(int resourceId)
        {
            var assignments = await _db.ResourceAssignments
                .Where(a => a.ResourceId == resourceId && a.IsActive)
                .Include(a => a.Resource)
                .Include(a => a.Volunteer)
                .Include(a => a.AssignedByUser)
                .Include(a => a.UsageLogs)
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();

            return assignments.Select(MapToDto);
        }

        public async Task<IEnumerable<ResourceAssignmentDto>> GetPendingAssignmentsAsync()
        {
            var assignments = await _db.ResourceAssignments
                .Where(a => a.Status == AssignmentStatus.Pending && a.IsActive)
                .Include(a => a.Resource)
                .Include(a => a.Volunteer)
                .Include(a => a.AssignedByUser)
                .Include(a => a.UsageLogs)
                .OrderBy(a => a.AssignedAt)
                .ToListAsync();

            return assignments.Select(MapToDto);
        }

        public async Task<IEnumerable<ResourceAssignmentDto>> GetOverdueReturnsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var assignments = await _db.ResourceAssignments
                .Where(a => a.Status == AssignmentStatus.InUse && 
                           a.IsActive && 
                           a.ExpectedReturnDate.HasValue && 
                           a.ExpectedReturnDate.Value.Date < today)
                .Include(a => a.Resource)
                .Include(a => a.Volunteer)
                .Include(a => a.AssignedByUser)
                .Include(a => a.UsageLogs)
                .OrderBy(a => a.ExpectedReturnDate)
                .ToListAsync();

            return assignments.Select(MapToDto);
        }

        public async Task<IEnumerable<ResourceAssignmentDto>> GetActiveAssignmentsAsync()
        {
            var assignments = await _db.ResourceAssignments
                .Where(a => (a.Status == AssignmentStatus.Pending || 
                            a.Status == AssignmentStatus.Confirmed || 
                            a.Status == AssignmentStatus.InUse) && a.IsActive)
                .Include(a => a.Resource)
                .Include(a => a.Volunteer)
                .Include(a => a.AssignedByUser)
                .Include(a => a.UsageLogs)
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();

            return assignments.Select(MapToDto);
        }

        public async Task<object> GetAssignmentStatisticsAsync()
        {
            var totalAssignments = await _db.ResourceAssignments.CountAsync(a => a.IsActive);
            var pendingCount = await _db.ResourceAssignments.CountAsync(a => a.Status == AssignmentStatus.Pending && a.IsActive);
            var confirmedCount = await _db.ResourceAssignments.CountAsync(a => a.Status == AssignmentStatus.Confirmed && a.IsActive);
            var inUseCount = await _db.ResourceAssignments.CountAsync(a => a.Status == AssignmentStatus.InUse && a.IsActive);
            var returnedCount = await _db.ResourceAssignments.CountAsync(a => a.Status == AssignmentStatus.Returned && a.IsActive);
            var cancelledCount = await _db.ResourceAssignments.CountAsync(a => a.Status == AssignmentStatus.Cancelled && a.IsActive);

            var today = DateTime.UtcNow.Date;
            var overdueCount = await _db.ResourceAssignments.CountAsync(a => 
                a.Status == AssignmentStatus.InUse && 
                a.IsActive && 
                a.ExpectedReturnDate.HasValue && 
                a.ExpectedReturnDate.Value.Date < today);

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

        private static ResourceAssignmentDto MapToDto(ResourceAssignment assignment)
        {
            return new ResourceAssignmentDto
            {
                Id = assignment.Id,
                ResourceId = assignment.ResourceId,
                ResourceName = assignment.Resource.Name,
                ResourceCategory = assignment.Resource.Category,
                VolunteerId = assignment.VolunteerId,
                VolunteerName = $"{assignment.Volunteer.FirstName} {assignment.Volunteer.LastName}",
                QuantityAssigned = assignment.QuantityAssigned,
                Status = assignment.Status,
                AssignedAt = assignment.AssignedAt,
                ConfirmedAt = assignment.ConfirmedAt,
                StartedAt = assignment.StartedAt,
                ReturnedAt = assignment.ReturnedAt,
                ExpectedReturnDate = assignment.ExpectedReturnDate,
                InitialNotes = assignment.InitialNotes,
                VolunteerNotes = assignment.VolunteerNotes,
                AssignedByUserId = assignment.AssignedByUserId,
                AssignedByUserName = assignment.AssignedByUser != null ? $"{assignment.AssignedByUser.FirstName} {assignment.AssignedByUser.LastName}" : null,
                IsActive = assignment.IsActive,
                UsageLogsCount = assignment.UsageLogs.Count,
                HasIncidents = assignment.UsageLogs.Any(l => l.EventType == UsageEventType.IncidentReport || l.EventType == UsageEventType.DamageReport)
            };
        }
    }
}