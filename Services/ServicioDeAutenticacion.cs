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
        Task<RespuestaInicioSesionDto?> LoginAsync(IniciarSesionDto loginDto);
        Task<UsuarioDto> RegisterAsync(RegistrarDto registerDto);
        Task<UsuarioDto?> GetCurrentUserAsync(int userId);
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
        //Mayerly Dionicio
        public async Task<RespuestaInicioSesionDto?> LoginAsync(IniciarSesionDto loginDto)
        {
            // Buscar usuario por email
            var user = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.CorreoElectronico == loginDto.Email && u.EstaActivo);

            if (user == null || !VerifyPassword(loginDto.Contrasena, user.HashDeContrasena))
                return null;

            // Generar token JWT
            var token = _jwtService.GenerateToken(user);

            return new RespuestaInicioSesionDto
            {
                Token = token,
                Expires = DateTime.UtcNow.AddMinutes(60), // Configurar según necesidades
                User = MapToDto(user)
            };
        }

        public async Task<UsuarioDto> RegisterAsync(RegistrarDto registerDto)
        {
            // Validar que el email no existe
            if (await _context.Usuarios.AnyAsync(u => u.CorreoElectronico == registerDto.Email && u.EstaActivo))
                throw new ArgumentException("Email already exists");

            // Validar que el username no existe
            if (await _context.Usuarios.AnyAsync(u => u.NombreDeUsuario == registerDto.NombreDeUsuario && u.EstaActivo))
                throw new ArgumentException("Username already exists");

            // Verificar que el rol existe
            var role = await _context.Roles.FindAsync(registerDto.RolId);
            if (role == null || !role.EstaActivo)
                throw new ArgumentException("Invalid role");

            // Validar fecha de nacimiento si es participante (RolId = 2)
            if (registerDto.RolId == 2 && !registerDto.FechaDeNacimiento.HasValue)
                throw new ArgumentException("Fecha de nacimiento es requerida para participantes");

            // Validar que la fecha de nacimiento sea válida
            if (registerDto.FechaDeNacimiento.HasValue)
            {
                if (registerDto.FechaDeNacimiento.Value > DateTime.Today)
                    throw new ArgumentException("La fecha de nacimiento no puede ser futura");
                    
                var edad = DateTime.Today.Year - registerDto.FechaDeNacimiento.Value.Year;
                if (registerDto.FechaDeNacimiento.Value.Date > DateTime.Today.AddYears(-edad))
                    edad--;
                    
                if (edad < 0 || edad > 120)
                    throw new ArgumentException("La fecha de nacimiento no es válida");
            }

            var user = new Usuario
            {
                PrimerNombre = registerDto.PrimerNombre,
                Apellido = registerDto.Apellido,
                CorreoElectronico = registerDto.Email,
                NombreDeUsuario = registerDto.NombreDeUsuario,
                HashDeContrasena = HashPassword(registerDto.Contrasena),
                IdRol = registerDto.RolId,
                CodigoParticipante = registerDto.CodigoParticipante,
                FechaDeNacimiento = registerDto.FechaDeNacimiento,
                CreadoEn = DateTime.UtcNow,
                EstaActivo = true
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            // Cargar el rol para el DTO
            await _context.Entry(user).Reference(u => u.Rol).LoadAsync();

            return MapToDto(user);
        }

        public async Task<UsuarioDto?> GetCurrentUserAsync(int userId)
        {
            var user = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == userId && u.EstaActivo);

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
                CodigoParticipante = user.CodigoParticipante,
                FechaDeNacimiento = user.FechaDeNacimiento,
                Edad = user.Edad,
                CreadoEn = user.CreadoEn,
                ActualizadoEn = user.ActualizadoEn,
                EstaActivo = user.EstaActivo
            };
        }
    }
}
