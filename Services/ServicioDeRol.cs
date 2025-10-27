using ApiPG.Models;
using ApiPG.DTOs;
using ApiPG.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiPG.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RolDto>> GetAllRolesAsync();
        Task<RolDto?> GetRoleByIdAsync(int id);
        Task<RolDto> CreateRoleAsync(CrearRolDto createRoleDto);
        Task<RolDto?> UpdateRoleAsync(int id, ActualizarRolDto updateRoleDto);
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

        public async Task<IEnumerable<RolDto>> GetAllRolesAsync()
        {
            var roles = await _context.Roles
                .Where(r => r.EstaActivo)
                .Include(r => r.Usuarios)
                .Select(r => MapToDto(r))
                .ToListAsync();

            return roles;
        }

        public async Task<RolDto?> GetRoleByIdAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.Usuarios)
                .FirstOrDefaultAsync(r => r.Id == id && r.EstaActivo);

            return role != null ? MapToDto(role) : null;
        }

        //Mayerly Dionicio
        public async Task<RolDto> CreateRoleAsync(CrearRolDto createRoleDto)
        {
            // Validar que el rol no existe
            if (await RoleExistsAsync(createRoleDto.Nombre))
                throw new ArgumentException($"Role with name '{createRoleDto.Nombre}' already exists");

            var role = new Rol
            {
                Nombre = createRoleDto.Nombre,
                Descripcion = createRoleDto.Descripcion,
                CreadoEn = DateTime.UtcNow,
                EstaActivo = true
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return MapToDto(role);
        }

        public async Task<RolDto?> UpdateRoleAsync(int id, ActualizarRolDto updateRoleDto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return null;

            // Validar que el nuevo nombre no existe (excluyendo el rol actual)
            if (!string.IsNullOrEmpty(updateRoleDto.Nombre))
            {
                if (await RoleExistsAsync(updateRoleDto.Nombre, id))
                    throw new ArgumentException($"Role with name '{updateRoleDto.Nombre}' already exists");
                
                role.Nombre = updateRoleDto.Nombre;
            }

            if (updateRoleDto.Descripcion != null)
                role.Descripcion = updateRoleDto.Descripcion;
            
            if (updateRoleDto.EstaActivo.HasValue)
                role.EstaActivo = updateRoleDto.EstaActivo.Value;

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

            role.EstaActivo = false; // Soft delete
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RoleExistsAsync(string name, int? excludeRoleId = null)
        {
            return await _context.Roles
                .AnyAsync(r => r.EstaActivo && 
                              r.Nombre.ToLower() == name.ToLower() &&
                              (!excludeRoleId.HasValue || r.Id != excludeRoleId.Value));
        }

        public async Task<bool> RoleHasUsersAsync(int roleId)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.IdRol == roleId && u.EstaActivo);
        }

        private static RolDto MapToDto(Rol role)
        {
            return new RolDto
            {
                Id = role.Id,
                Nombre = role.Nombre,
                Descripcion = role.Descripcion,
                CreadoEn = role.CreadoEn,
                EstaActivo = role.EstaActivo,
                CantidadUsuarios = role.Usuarios?.Count(u => u.EstaActivo) ?? 0
            };
        }
    }
}
