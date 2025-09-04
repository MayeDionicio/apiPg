using ApiPG.Models;
using ApiPG.DTOs;
using ApiPG.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiPG.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int id);
        Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto);
        Task<bool> DeleteRoleAsync(int id);
        Task<bool> RoleExistsAsync(string name, int? excludeRoleId = null);
        Task<bool> RoleHasUsersAsync(int roleId);
    }

    public class RoleService : IRoleService
    {
        private readonly ApiPGContext _context;

        public RoleService(ApiPGContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _context.Roles
                .Where(r => r.IsActive)
                .Include(r => r.Users)
                .Select(r => MapToDto(r))
                .ToListAsync();

            return roles;
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);

            return role != null ? MapToDto(role) : null;
        }

        public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            // Validar que el rol no existe
            if (await RoleExistsAsync(createRoleDto.Name))
                throw new ArgumentException($"Role with name '{createRoleDto.Name}' already exists");

            var role = new Role
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return MapToDto(role);
        }

        public async Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return null;

            // Validar que el nuevo nombre no existe (excluyendo el rol actual)
            if (!string.IsNullOrEmpty(updateRoleDto.Name))
            {
                if (await RoleExistsAsync(updateRoleDto.Name, id))
                    throw new ArgumentException($"Role with name '{updateRoleDto.Name}' already exists");
                
                role.Name = updateRoleDto.Name;
            }

            if (updateRoleDto.Description != null)
                role.Description = updateRoleDto.Description;
            
            if (updateRoleDto.IsActive.HasValue)
                role.IsActive = updateRoleDto.IsActive.Value;

            await _context.SaveChangesAsync();
            return MapToDto(role);
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return false;

            // Verificar si el rol tiene usuarios asignados
            if (await RoleHasUsersAsync(id))
                throw new InvalidOperationException("Cannot delete role because it has users assigned to it");

            role.IsActive = false; // Soft delete
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RoleExistsAsync(string name, int? excludeRoleId = null)
        {
            return await _context.Roles
                .AnyAsync(r => r.IsActive && 
                              r.Name.ToLower() == name.ToLower() &&
                              (!excludeRoleId.HasValue || r.Id != excludeRoleId.Value));
        }

        public async Task<bool> RoleHasUsersAsync(int roleId)
        {
            return await _context.Users
                .AnyAsync(u => u.RoleId == roleId && u.IsActive);
        }

        private static RoleDto MapToDto(Role role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                IsActive = role.IsActive,
                UsersCount = role.Users?.Count(u => u.IsActive) ?? 0
            };
        }
    }
}
