using ApiPG.Models;
using ApiPG.DTOs;
using ApiPG.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ApiPG.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UsuarioDto>> GetAllUsersAsync();
        Task<UsuarioDto?> GetUserByIdAsync(int id);
        Task<UsuarioDto?> GetUserByUsernameAsync(string username);
        Task<UsuarioDto> CreateUserAsync(CrearUsuarioDto createUserDto);
        Task<UsuarioDto?> UpdateUserAsync(int id, ActualizarUsuarioDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(string username, string email, int? excludeUserId = null);
    }

    public class UserService : IUserService
    {
        private readonly ApiPGContext _context;

        public UserService(ApiPGContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsuarioDto>> GetAllUsersAsync()
        {
            var users = await _context.Usuarios
                .Where(u => u.EstaActivo)
                .Include(u => u.Rol)
                .Select(u => MapToDto(u))
                .ToListAsync();

            return users;
        }

        public async Task<UsuarioDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == id && u.EstaActivo);

            return user != null ? MapToDto(user) : null;
        }

        public async Task<UsuarioDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.NombreDeUsuario == username && u.EstaActivo);

            return user != null ? MapToDto(user) : null;
        }

        public async Task<UsuarioDto> CreateUserAsync(CrearUsuarioDto createUserDto)
        {
            // Validar que el rol existe
            var role = await _context.Roles.FindAsync(createUserDto.RolId);
            if (role == null || !role.EstaActivo)
                throw new ArgumentException($"Role with ID {createUserDto.RolId} not found or inactive");

            // Validar que el usuario no existe
            if (await UserExistsAsync(createUserDto.NombreDeUsuario, createUserDto.Email))
                throw new ArgumentException("Username or email already exists");

            var user = new Usuario
            {
                PrimerNombre = createUserDto.PrimerNombre,
                Apellido = createUserDto.Apellido,
                CorreoElectronico = createUserDto.Email,
                NombreDeUsuario = createUserDto.NombreDeUsuario,
                HashDeContrasena = HashPassword(createUserDto.Contrasena),
                IdRol = createUserDto.RolId,
                CreadoEn = DateTime.UtcNow,
                EstaActivo = true
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            // Cargar el rol para el DTO
            await _context.Entry(user).Reference(u => u.Rol).LoadAsync();
            
            return MapToDto(user);
        }

        public async Task<UsuarioDto?> UpdateUserAsync(int id, ActualizarUsuarioDto updateUserDto)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user == null) return null;

            // Validar que el nuevo username/email no existe (excluyendo el usuario actual)
            if (!string.IsNullOrEmpty(updateUserDto.NombreDeUsuario) || !string.IsNullOrEmpty(updateUserDto.Email))
            {
                var username = updateUserDto.NombreDeUsuario ?? user.NombreDeUsuario;
                var email = updateUserDto.Email ?? user.CorreoElectronico;
                
                if (await UserExistsAsync(username, email, id))
                    throw new ArgumentException("Username or email already exists");
            }

            // Validar rol si se proporciona
            if (updateUserDto.RolId.HasValue)
            {
                var role = await _context.Roles.FindAsync(updateUserDto.RolId.Value);
                if (role == null || !role.EstaActivo)
                    throw new ArgumentException($"Role with ID {updateUserDto.RolId} not found or inactive");
                
                user.IdRol = updateUserDto.RolId.Value;
            }

            // Actualizar campos
            if (!string.IsNullOrEmpty(updateUserDto.PrimerNombre))
                user.PrimerNombre = updateUserDto.PrimerNombre;
            
            if (!string.IsNullOrEmpty(updateUserDto.Apellido))
                user.Apellido = updateUserDto.Apellido;
            
            if (!string.IsNullOrEmpty(updateUserDto.Email))
                user.CorreoElectronico = updateUserDto.Email;
            
            if (!string.IsNullOrEmpty(updateUserDto.NombreDeUsuario))
                user.NombreDeUsuario = updateUserDto.NombreDeUsuario;
            
            if (!string.IsNullOrEmpty(updateUserDto.Contrasena))
                user.HashDeContrasena = HashPassword(updateUserDto.Contrasena);
            
            if (updateUserDto.EstaActivo.HasValue)
                user.EstaActivo = updateUserDto.EstaActivo.Value;

            user.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Cargar el rol para el DTO
            await _context.Entry(user).Reference(u => u.Rol).LoadAsync();
            
            return MapToDto(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user == null) return false;

            user.EstaActivo = false; // Soft delete
            user.ActualizadoEn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExistsAsync(string username, string email, int? excludeUserId = null)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.EstaActivo && 
                              (u.NombreDeUsuario == username || u.CorreoElectronico == email) &&
                              (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
        }

        private static UsuarioDto MapToDto(Usuario user)
        {
            return new UsuarioDto
            {
                Id = user.Id,
                PrimerNombre = user.PrimerNombre,
                Apellido = user.Apellido,
                NombreCompleto = user.NombreCompleto,
                Email = user.CorreoElectronico,
                NombreDeUsuario = user.NombreDeUsuario,
                RolId = user.IdRol,
                NombreRol = user.Rol?.Nombre ?? "Unknown",
                CreadoEn = user.CreadoEn,
                ActualizadoEn = user.ActualizadoEn,
                EstaActivo = user.EstaActivo
            };
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "ApiPG_Salt"));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
