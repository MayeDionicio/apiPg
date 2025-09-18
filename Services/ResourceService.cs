using ApiPG.DTOs;
using ApiPG.Data;
using ApiPG.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPG.Services
{
    public class ResourceService : IResourceService
    {
        private readonly ApiPGContext _db;

        public ResourceService(ApiPGContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ResourceDto>> GetAllAsync()
        {
            var resources = await _db.Resources
                .Include(r => r.ResourceAssignments)
                .OrderBy(r => r.Name)
                .ToListAsync();

            return resources.Select(MapToDto);
        }

        public async Task<IEnumerable<ResourceDto>> GetActiveAsync()
        {
            var resources = await _db.Resources
                .Where(r => r.IsActive)
                .Include(r => r.ResourceAssignments)
                .OrderBy(r => r.Name)
                .ToListAsync();

            return resources.Select(MapToDto);
        }

        public async Task<ResourceDto?> GetByIdAsync(int id)
        {
            var resource = await _db.Resources
                .Include(r => r.ResourceAssignments)
                .FirstOrDefaultAsync(r => r.Id == id);

            return resource != null ? MapToDto(resource) : null;
        }

        public async Task<ResourceDto> CreateAsync(CreateResourceDto dto)
        {
            var resource = new Resource
            {
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                Quantity = dto.Quantity,
                AvailableQuantity = dto.Quantity, // Inicialmente toda la cantidad está disponible
                Unit = dto.Unit,
                Location = dto.Location,
                EstimatedValue = dto.EstimatedValue,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _db.Resources.Add(resource);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(resource.Id) ?? throw new InvalidOperationException("Error al crear el recurso");
        }

        public async Task<ResourceDto?> UpdateAsync(int id, UpdateResourceDto dto)
        {
            var resource = await _db.Resources.FindAsync(id);
            if (resource == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                resource.Name = dto.Name;

            if (dto.Description != null)
                resource.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.Category))
                resource.Category = dto.Category;

            if (dto.Quantity.HasValue)
            {
                var quantityDiff = dto.Quantity.Value - resource.Quantity;
                resource.Quantity = dto.Quantity.Value;
                resource.AvailableQuantity = Math.Max(0, resource.AvailableQuantity + quantityDiff);
            }

            if (dto.Unit != null)
                resource.Unit = dto.Unit;

            if (dto.Location != null)
                resource.Location = dto.Location;

            if (dto.EstimatedValue.HasValue)
                resource.EstimatedValue = dto.EstimatedValue.Value;

            if (dto.Notes != null)
                resource.Notes = dto.Notes;

            if (dto.IsActive.HasValue)
                resource.IsActive = dto.IsActive.Value;

            resource.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var resource = await _db.Resources.FindAsync(id);
            if (resource == null) return false;

            resource.IsActive = false;
            resource.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateAsync(int id)
        {
            var resource = await _db.Resources.FindAsync(id);
            if (resource == null) return false;

            resource.IsActive = true;
            resource.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ResourceDto>> GetByCategoryAsync(string category)
        {
            var resources = await _db.Resources
                .Where(r => r.IsActive && r.Category.ToLower() == category.ToLower())
                .Include(r => r.ResourceAssignments)
                .OrderBy(r => r.Name)
                .ToListAsync();

            return resources.Select(MapToDto);
        }

        public async Task<IEnumerable<ResourceDto>> GetAvailableAsync()
        {
            var resources = await _db.Resources
                .Where(r => r.IsActive && r.AvailableQuantity > 0)
                .Include(r => r.ResourceAssignments)
                .OrderBy(r => r.Name)
                .ToListAsync();

            return resources.Select(MapToDto);
        }

        public async Task<IEnumerable<ResourceDto>> GetLowStockAsync(int threshold = 5)
        {
            var resources = await _db.Resources
                .Where(r => r.IsActive && r.AvailableQuantity <= threshold)
                .Include(r => r.ResourceAssignments)
                .OrderBy(r => r.AvailableQuantity)
                .ToListAsync();

            return resources.Select(MapToDto);
        }

        public async Task<bool> UpdateQuantityAsync(int resourceId, int newQuantity)
        {
            var resource = await _db.Resources.FindAsync(resourceId);
            if (resource == null) return false;

            var quantityDiff = newQuantity - resource.Quantity;
            resource.Quantity = newQuantity;
            resource.AvailableQuantity = Math.Max(0, resource.AvailableQuantity + quantityDiff);
            resource.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReserveQuantityAsync(int resourceId, int quantity)
        {
            var resource = await _db.Resources.FindAsync(resourceId);
            if (resource == null || resource.AvailableQuantity < quantity) return false;

            resource.AvailableQuantity -= quantity;
            resource.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReleaseQuantityAsync(int resourceId, int quantity)
        {
            var resource = await _db.Resources.FindAsync(resourceId);
            if (resource == null) return false;

            resource.AvailableQuantity = Math.Min(resource.Quantity, resource.AvailableQuantity + quantity);
            resource.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<object> GetResourceStatisticsAsync()
        {
            var totalResources = await _db.Resources.CountAsync(r => r.IsActive);
            var totalAvailable = await _db.Resources.Where(r => r.IsActive).SumAsync(r => r.AvailableQuantity);
            var totalAssigned = await _db.Resources.Where(r => r.IsActive).SumAsync(r => r.Quantity - r.AvailableQuantity);
            var lowStockCount = await _db.Resources.CountAsync(r => r.IsActive && r.AvailableQuantity <= 5);
            var categoriesCount = await _db.Resources.Where(r => r.IsActive).Select(r => r.Category).Distinct().CountAsync();

            var categoryStats = await _db.Resources
                .Where(r => r.IsActive)
                .GroupBy(r => r.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Count = g.Count(),
                    TotalQuantity = g.Sum(r => r.Quantity),
                    AvailableQuantity = g.Sum(r => r.AvailableQuantity),
                    EstimatedValue = g.Sum(r => r.EstimatedValue ?? 0)
                })
                .ToListAsync();

            return new
            {
                TotalResources = totalResources,
                TotalAvailable = totalAvailable,
                TotalAssigned = totalAssigned,
                LowStockCount = lowStockCount,
                CategoriesCount = categoriesCount,
                CategoryStatistics = categoryStats
            };
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _db.Resources
                .Where(r => r.IsActive)
                .Select(r => r.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        private static ResourceDto MapToDto(Resource resource)
        {
            return new ResourceDto
            {
                Id = resource.Id,
                Name = resource.Name,
                Description = resource.Description,
                Category = resource.Category,
                Quantity = resource.Quantity,
                AvailableQuantity = resource.AvailableQuantity,
                Unit = resource.Unit,
                Location = resource.Location,
                EstimatedValue = resource.EstimatedValue,
                Notes = resource.Notes,
                CreatedAt = resource.CreatedAt,
                UpdatedAt = resource.UpdatedAt,
                IsActive = resource.IsActive,
                AssignmentsCount = resource.ResourceAssignments.Count,
                ActiveAssignmentsCount = resource.ResourceAssignments.Count(a => a.IsActive && a.Status != AssignmentStatus.Returned && a.Status != AssignmentStatus.Cancelled)
            };
        }
    }
}