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

        public async Task<IEnumerable<RecursoDto>> GetAllAsync()
        {
            var resources = await _db.Recursos
                .Include(r => r.AsignacionesDeRecurso)
                .OrderBy(r => r.Nombre)
                .ToListAsync();

            return resources.Select(MapToDto);
        }

        public async Task<IEnumerable<RecursoDto>> GetActiveAsync()
        {
            var resources = await _db.Recursos
                .Where(r => r.EstaActivo)
                .Include(r => r.AsignacionesDeRecurso)
                .OrderBy(r => r.Nombre)
                .ToListAsync();

            return resources.Select(MapToDto);
        }

        public async Task<RecursoDto?> GetByIdAsync(int id)
        {
            var resource = await _db.Recursos
                .Include(r => r.AsignacionesDeRecurso)
                .FirstOrDefaultAsync(r => r.Id == id);

            return resource != null ? MapToDto(resource) : null;
        }

        public async Task<RecursoDto> CreateAsync(CrearRecursoDto dto)
        {
            var resource = new Recurso
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Categoria = dto.Categoria,
                Cantidad = dto.Cantidad,
                CantidadDisponible = dto.Cantidad, // Inicialmente toda la cantidad está disponible
                Unidad = dto.Unidad,
                Ubicacion = dto.Ubicacion,
                ValorEstimado = dto.ValorEstimado,
                Notas = dto.Notas,
                CreadoEn = DateTime.UtcNow,
                EstaActivo = true
            };

            _db.Recursos.Add(resource);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(resource.Id) ?? throw new InvalidOperationException("Error al crear el recurso");
        }

        public async Task<RecursoDto?> UpdateAsync(int id, ActualizarRecursoDto dto)
        {
            var resource = await _db.Recursos.FindAsync(id);
            if (resource == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                resource.Nombre = dto.Nombre;

            if (dto.Descripcion != null)
                resource.Descripcion = dto.Descripcion;

            if (!string.IsNullOrWhiteSpace(dto.Categoria))
                resource.Categoria = dto.Categoria;

            if (dto.Cantidad.HasValue)
            {
                var quantityDiff = dto.Cantidad.Value - resource.Cantidad;
                resource.Cantidad = dto.Cantidad.Value;
                resource.CantidadDisponible = Math.Max(0, resource.CantidadDisponible + quantityDiff);
            }

            if (dto.Unidad != null)
                resource.Unidad = dto.Unidad;

            if (dto.Ubicacion != null)
                resource.Ubicacion = dto.Ubicacion;

            if (dto.ValorEstimado.HasValue)
                resource.ValorEstimado = dto.ValorEstimado.Value;

            if (dto.Notas != null)
                resource.Notas = dto.Notas;

            if (dto.EstaActivo.HasValue)
                resource.EstaActivo = dto.EstaActivo.Value;

            resource.ActualizadoEn = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var resource = await _db.Recursos.FindAsync(id);
            if (resource == null) return false;

            resource.EstaActivo = false;
            resource.ActualizadoEn = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateAsync(int id)
        {
            var resource = await _db.Recursos.FindAsync(id);
            if (resource == null) return false;

            resource.EstaActivo = true;
            resource.ActualizadoEn = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RecursoDto>> GetByCategoryAsync(string category)
        {
            var resources = await _db.Recursos
                .Where(r => r.EstaActivo && r.Categoria.ToLower() == category.ToLower())
                .Include(r => r.AsignacionesDeRecurso)
                .OrderBy(r => r.Nombre)
                .ToListAsync();

            return resources.Select(MapToDto);
        }

        public async Task<IEnumerable<RecursoDto>> GetAvailableAsync()
        {
            var resources = await _db.Recursos
                .Where(r => r.EstaActivo && r.CantidadDisponible > 0)
                .Include(r => r.AsignacionesDeRecurso)
                .OrderBy(r => r.Nombre)
                .ToListAsync();

            return resources.Select(MapToDto);
        }

        public async Task<IEnumerable<RecursoDto>> GetLowStockAsync(int threshold = 5)
        {
            var resources = await _db.Recursos
                .Where(r => r.EstaActivo && r.CantidadDisponible <= threshold)
                .Include(r => r.AsignacionesDeRecurso)
                .OrderBy(r => r.CantidadDisponible)
                .ToListAsync();

            return resources.Select(MapToDto);
        }

        public async Task<bool> UpdateQuantityAsync(int resourceId, int newQuantity)
        {
            var resource = await _db.Recursos.FindAsync(resourceId);
            if (resource == null) return false;

            var quantityDiff = newQuantity - resource.Cantidad;
            resource.Cantidad = newQuantity;
            resource.CantidadDisponible = Math.Max(0, resource.CantidadDisponible + quantityDiff);
            resource.ActualizadoEn = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReserveQuantityAsync(int resourceId, int quantity)
        {
            var resource = await _db.Recursos.FindAsync(resourceId);
            if (resource == null || resource.CantidadDisponible < quantity) return false;

            resource.CantidadDisponible -= quantity;
            resource.ActualizadoEn = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReleaseQuantityAsync(int resourceId, int quantity)
        {
            var resource = await _db.Recursos.FindAsync(resourceId);
            if (resource == null) return false;

            resource.CantidadDisponible = Math.Min(resource.Cantidad, resource.CantidadDisponible + quantity);
            resource.ActualizadoEn = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<object> GetResourceStatisticsAsync()
        {
            var totalResources = await _db.Recursos.CountAsync(r => r.EstaActivo);
            var totalAvailable = await _db.Recursos.Where(r => r.EstaActivo).SumAsync(r => r.CantidadDisponible);
            var totalAssigned = await _db.Recursos.Where(r => r.EstaActivo).SumAsync(r => r.Cantidad - r.CantidadDisponible);
            var lowStockCount = await _db.Recursos.CountAsync(r => r.EstaActivo && r.CantidadDisponible <= 5);
            var categoriesCount = await _db.Recursos.Where(r => r.EstaActivo).Select(r => r.Categoria).Distinct().CountAsync();

            var categoryStats = await _db.Recursos
                .Where(r => r.EstaActivo)
                .GroupBy(r => r.Categoria)
                .Select(g => new
                {
                    Category = g.Key,
                    Count = g.Count(),
                    TotalQuantity = g.Sum(r => r.Cantidad),
                    AvailableQuantity = g.Sum(r => r.CantidadDisponible),
                    EstimatedValue = g.Sum(r => r.ValorEstimado ?? 0)
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
            return await _db.Recursos
                .Where(r => r.EstaActivo)
                .Select(r => r.Categoria)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        private static RecursoDto MapToDto(Recurso resource)
        {
            return new RecursoDto
            {
                Id = resource.Id,
                Nombre = resource.Nombre,
                Descripcion = resource.Descripcion,
                Categoria = resource.Categoria,
                Cantidad = resource.Cantidad,
                CantidadDisponible = resource.CantidadDisponible,
                Unidad = resource.Unidad,
                Ubicacion = resource.Ubicacion,
                ValorEstimado = resource.ValorEstimado,
                Notas = resource.Notas,
                CreadoEn = resource.CreadoEn,
                ActualizadoEn = resource.ActualizadoEn,
                EstaActivo = resource.EstaActivo,
                CantidadAsignaciones = resource.AsignacionesDeRecurso.Count,
                CantidadAsignacionesActivas = resource.AsignacionesDeRecurso.Count(a => a.EstaActivo && a.Estado != EstadoDeAsignacion.Devuelto && a.Estado != EstadoDeAsignacion.Cancelado)
            };
        }
    }
}
