using ApiPG.Models;
using ApiPG.DTOs;
using ApiPG.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ApiPG.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
        Task<UserDto> RegisterAsync(RegisterDto registerDto);
        Task<UserDto?> GetCurrentUserAsync(int userId);
    }

    public class AuthService : IAuthService
    {
        private readonly ApiPGContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(ApiPGContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // Buscar usuario por email
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsActive);

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                return null;

            // Generar token JWT
            var token = _jwtService.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                Expires = DateTime.UtcNow.AddMinutes(60), // Configurar según necesidades
                User = MapToDto(user)
            };
        }

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            // Validar que el email no existe
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email && u.IsActive))
                throw new ArgumentException("Email already exists");

            // Validar que el username no existe
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username && u.IsActive))
                throw new ArgumentException("Username already exists");

            // Verificar que el rol existe
            var role = await _context.Roles.FindAsync(registerDto.RoleId);
            if (role == null || !role.IsActive)
                throw new ArgumentException("Invalid role");

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                Username = registerDto.Username,
                PasswordHash = HashPassword(registerDto.Password),
                RoleId = registerDto.RoleId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Cargar el rol para el DTO
            await _context.Entry(user).Reference(u => u.Role).LoadAsync();

            return MapToDto(user);
        }

        public async Task<UserDto?> GetCurrentUserAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            return user != null ? MapToDto(user) : null;
        }

        private static bool VerifyPassword(string password, string hash)
        {
            var computedHash = HashPassword(password);
            return computedHash == hash;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "ApiPG_Salt"));
            return Convert.ToBase64String(hashedBytes);
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
    }
}
