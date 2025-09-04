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
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
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

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .Include(u => u.Role)
                .Select(u => MapToDto(u))
                .ToListAsync();

            return users;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Validar que el rol existe
            var role = await _context.Roles.FindAsync(createUserDto.RoleId);
            if (role == null || !role.IsActive)
                throw new ArgumentException($"Role with ID {createUserDto.RoleId} not found or inactive");

            // Validar que el usuario no existe
            if (await UserExistsAsync(createUserDto.Username, createUserDto.Email))
                throw new ArgumentException("Username or email already exists");

            var user = new User
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                Username = createUserDto.Username,
                PasswordHash = HashPassword(createUserDto.Password),
                RoleId = createUserDto.RoleId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Cargar el rol para el DTO
            await _context.Entry(user).Reference(u => u.Role).LoadAsync();
            
            return MapToDto(user);
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            // Validar que el nuevo username/email no existe (excluyendo el usuario actual)
            if (!string.IsNullOrEmpty(updateUserDto.Username) || !string.IsNullOrEmpty(updateUserDto.Email))
            {
                var username = updateUserDto.Username ?? user.Username;
                var email = updateUserDto.Email ?? user.Email;
                
                if (await UserExistsAsync(username, email, id))
                    throw new ArgumentException("Username or email already exists");
            }

            // Validar rol si se proporciona
            if (updateUserDto.RoleId.HasValue)
            {
                var role = await _context.Roles.FindAsync(updateUserDto.RoleId.Value);
                if (role == null || !role.IsActive)
                    throw new ArgumentException($"Role with ID {updateUserDto.RoleId} not found or inactive");
                
                user.RoleId = updateUserDto.RoleId.Value;
            }

            // Actualizar campos
            if (!string.IsNullOrEmpty(updateUserDto.FirstName))
                user.FirstName = updateUserDto.FirstName;
            
            if (!string.IsNullOrEmpty(updateUserDto.LastName))
                user.LastName = updateUserDto.LastName;
            
            if (!string.IsNullOrEmpty(updateUserDto.Email))
                user.Email = updateUserDto.Email;
            
            if (!string.IsNullOrEmpty(updateUserDto.Username))
                user.Username = updateUserDto.Username;
            
            if (!string.IsNullOrEmpty(updateUserDto.Password))
                user.PasswordHash = HashPassword(updateUserDto.Password);
            
            if (updateUserDto.IsActive.HasValue)
                user.IsActive = updateUserDto.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Cargar el rol para el DTO
            await _context.Entry(user).Reference(u => u.Role).LoadAsync();
            
            return MapToDto(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.IsActive = false; // Soft delete
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExistsAsync(string username, string email, int? excludeUserId = null)
        {
            return await _context.Users
                .AnyAsync(u => u.IsActive && 
                              (u.Username == username || u.Email == email) &&
                              (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email,
                Username = user.Username,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? "Unknown",
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive
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
