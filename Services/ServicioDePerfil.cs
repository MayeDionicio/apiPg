using ApiPG.Data;
using ApiPG.DTOs;
using ApiPG.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPG.Services
{
    public interface IServicioDePerfil
    {
        Task<PerfilUsuarioDto?> ObtenerPerfilPorUsuarioIdAsync(int usuarioId);
        Task<PerfilUsuarioDto> CrearPerfilAsync(int usuarioId, CrearPerfilDto dto);
        Task<PerfilUsuarioDto?> ActualizarPerfilAsync(int usuarioId, ActualizarPerfilDto dto);
        Task<string> GuardarFotoPerfilAsync(int usuarioId, SubirFotoPerfilDto dto);
        Task<bool> EliminarFotoPerfilAsync(int usuarioId);
    }

    public class ServicioDePerfil : IServicioDePerfil
    {
        private readonly ApiPGContext _db;
        private readonly IWebHostEnvironment _env;

        public ServicioDePerfil(ApiPGContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<PerfilUsuarioDto?> ObtenerPerfilPorUsuarioIdAsync(int usuarioId)
        {
            var perfil = await _db.PerfilesUsuario
                .Include(p => p.Usuario)
                .ThenInclude(u => u.Rol)
                .FirstOrDefaultAsync(p => p.IdUsuario == usuarioId);

            if (perfil == null)
            {
                // Si no existe perfil, verificar si el usuario existe
                var usuario = await _db.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.Id == usuarioId);

                if (usuario == null)
                    return null;

                // Crear perfil automáticamente
                var nuevoPerfil = new PerfilUsuario
                {
                    IdUsuario = usuarioId,
                    CreadoEn = DateTime.UtcNow,
                    EstaActivo = true
                };

                _db.PerfilesUsuario.Add(nuevoPerfil);
                await _db.SaveChangesAsync();

                perfil = await _db.PerfilesUsuario
                    .Include(p => p.Usuario)
                    .ThenInclude(u => u.Rol)
                    .FirstOrDefaultAsync(p => p.Id == nuevoPerfil.Id);
            }

            if (perfil == null) return null;

            return MapearADto(perfil);
        }

        public async Task<PerfilUsuarioDto> CrearPerfilAsync(int usuarioId, CrearPerfilDto dto)
        {
            var usuario = await _db.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
                throw new ArgumentException("Usuario no encontrado");

            var perfilExistente = await _db.PerfilesUsuario
                .FirstOrDefaultAsync(p => p.IdUsuario == usuarioId);

            if (perfilExistente != null)
                throw new InvalidOperationException("El usuario ya tiene un perfil");

            var perfil = new PerfilUsuario
            {
                IdUsuario = usuarioId,
                FotoPerfil = dto.FotoPerfil,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                Ciudad = dto.Ciudad,
                Pais = dto.Pais,
                ContactoEmergenciaNombre = dto.ContactoEmergenciaNombre,
                ContactoEmergenciaTelefono = dto.ContactoEmergenciaTelefono,
                ContactoEmergenciaRelacion = dto.ContactoEmergenciaRelacion,
                NombrePadre = dto.NombrePadre,
                NombreMadre = dto.NombreMadre,
                NombreTutor = dto.NombreTutor,
                Alergias = dto.Alergias,
                CondicionesMedicas = dto.CondicionesMedicas,
                Medicamentos = dto.Medicamentos,
                Biografia = dto.Biografia,
                Intereses = dto.Intereses,
                Facebook = dto.Facebook,
                Instagram = dto.Instagram,
                Twitter = dto.Twitter,
                CreadoEn = DateTime.UtcNow,
                EstaActivo = true
            };

            _db.PerfilesUsuario.Add(perfil);
            await _db.SaveChangesAsync();

            var perfilCreado = await _db.PerfilesUsuario
                .Include(p => p.Usuario)
                .ThenInclude(u => u.Rol)
                .FirstOrDefaultAsync(p => p.Id == perfil.Id);

            return MapearADto(perfilCreado!);
        }

        public async Task<PerfilUsuarioDto?> ActualizarPerfilAsync(int usuarioId, ActualizarPerfilDto dto)
        {
            var perfil = await _db.PerfilesUsuario
                .Include(p => p.Usuario)
                .ThenInclude(u => u.Rol)
                .FirstOrDefaultAsync(p => p.IdUsuario == usuarioId);

            if (perfil == null)
            {
                // Crear perfil si no existe
                return await CrearPerfilAsync(usuarioId, new CrearPerfilDto
                {
                    FotoPerfil = dto.FotoPerfil,
                    Telefono = dto.Telefono,
                    Direccion = dto.Direccion,
                    Ciudad = dto.Ciudad,
                    Pais = dto.Pais,
                    ContactoEmergenciaNombre = dto.ContactoEmergenciaNombre,
                    ContactoEmergenciaTelefono = dto.ContactoEmergenciaTelefono,
                    ContactoEmergenciaRelacion = dto.ContactoEmergenciaRelacion,
                    NombrePadre = dto.NombrePadre,
                    NombreMadre = dto.NombreMadre,
                    NombreTutor = dto.NombreTutor,
                    Alergias = dto.Alergias,
                    CondicionesMedicas = dto.CondicionesMedicas,
                    Medicamentos = dto.Medicamentos,
                    Biografia = dto.Biografia,
                    Intereses = dto.Intereses,
                    Facebook = dto.Facebook,
                    Instagram = dto.Instagram,
                    Twitter = dto.Twitter
                });
            }

            // Actualizar solo los campos que no son null
            if (dto.FotoPerfil != null) perfil.FotoPerfil = dto.FotoPerfil;
            if (dto.Telefono != null) perfil.Telefono = dto.Telefono;
            if (dto.Direccion != null) perfil.Direccion = dto.Direccion;
            if (dto.Ciudad != null) perfil.Ciudad = dto.Ciudad;
            if (dto.Pais != null) perfil.Pais = dto.Pais;
            if (dto.ContactoEmergenciaNombre != null) perfil.ContactoEmergenciaNombre = dto.ContactoEmergenciaNombre;
            if (dto.ContactoEmergenciaTelefono != null) perfil.ContactoEmergenciaTelefono = dto.ContactoEmergenciaTelefono;
            if (dto.ContactoEmergenciaRelacion != null) perfil.ContactoEmergenciaRelacion = dto.ContactoEmergenciaRelacion;
            if (dto.NombrePadre != null) perfil.NombrePadre = dto.NombrePadre;
            if (dto.NombreMadre != null) perfil.NombreMadre = dto.NombreMadre;
            if (dto.NombreTutor != null) perfil.NombreTutor = dto.NombreTutor;
            if (dto.Alergias != null) perfil.Alergias = dto.Alergias;
            if (dto.CondicionesMedicas != null) perfil.CondicionesMedicas = dto.CondicionesMedicas;
            if (dto.Medicamentos != null) perfil.Medicamentos = dto.Medicamentos;
            if (dto.Biografia != null) perfil.Biografia = dto.Biografia;
            if (dto.Intereses != null) perfil.Intereses = dto.Intereses;
            if (dto.Facebook != null) perfil.Facebook = dto.Facebook;
            if (dto.Instagram != null) perfil.Instagram = dto.Instagram;
            if (dto.Twitter != null) perfil.Twitter = dto.Twitter;

            perfil.ActualizadoEn = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return MapearADto(perfil);
        }

        public async Task<string> GuardarFotoPerfilAsync(int usuarioId, SubirFotoPerfilDto dto)
        {
            var perfil = await _db.PerfilesUsuario
                .FirstOrDefaultAsync(p => p.IdUsuario == usuarioId);

            if (perfil == null)
            {
                // Crear perfil si no existe
                perfil = new PerfilUsuario
                {
                    IdUsuario = usuarioId,
                    CreadoEn = DateTime.UtcNow,
                    EstaActivo = true
                };
                _db.PerfilesUsuario.Add(perfil);
            }

            // Eliminar foto anterior si existe
            if (!string.IsNullOrEmpty(perfil.FotoPerfil))
            {
                var rutaAnterior = Path.Combine(_env.WebRootPath, perfil.FotoPerfil.TrimStart('/'));
                if (File.Exists(rutaAnterior))
                {
                    File.Delete(rutaAnterior);
                }
            }

            // Guardar nueva foto
            var bytes = Convert.FromBase64String(dto.FotoBase64);
            var extension = ".jpg";
            
            if (!string.IsNullOrEmpty(dto.NombreArchivo))
            {
                extension = Path.GetExtension(dto.NombreArchivo);
            }

            var nombreArchivo = $"perfil_{usuarioId}_{DateTime.UtcNow.Ticks}{extension}";
            var carpetaUploads = Path.Combine(_env.WebRootPath, "uploads", "perfiles");
            
            // Crear carpeta si no existe
            if (!Directory.Exists(carpetaUploads))
            {
                Directory.CreateDirectory(carpetaUploads);
            }

            var rutaCompleta = Path.Combine(carpetaUploads, nombreArchivo);
            await File.WriteAllBytesAsync(rutaCompleta, bytes);

            var urlFoto = $"/uploads/perfiles/{nombreArchivo}";
            perfil.FotoPerfil = urlFoto;
            perfil.ActualizadoEn = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return urlFoto;
        }

        public async Task<bool> EliminarFotoPerfilAsync(int usuarioId)
        {
            var perfil = await _db.PerfilesUsuario
                .FirstOrDefaultAsync(p => p.IdUsuario == usuarioId);

            if (perfil == null || string.IsNullOrEmpty(perfil.FotoPerfil))
                return false;

            var rutaArchivo = Path.Combine(_env.WebRootPath, perfil.FotoPerfil.TrimStart('/'));
            
            if (File.Exists(rutaArchivo))
            {
                File.Delete(rutaArchivo);
            }

            perfil.FotoPerfil = null;
            perfil.ActualizadoEn = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return true;
        }

        private PerfilUsuarioDto MapearADto(PerfilUsuario perfil)
        {
            return new PerfilUsuarioDto
            {
                Id = perfil.Id,
                IdUsuario = perfil.IdUsuario,
                NombreCompleto = perfil.Usuario.NombreCompleto,
                Email = perfil.Usuario.CorreoElectronico,
                NombreRol = perfil.Usuario.Rol.Nombre,
                FechaDeNacimiento = perfil.Usuario.FechaDeNacimiento,
                Edad = perfil.Usuario.Edad,
                FotoPerfil = perfil.FotoPerfil,
                Telefono = perfil.Telefono,
                Direccion = perfil.Direccion,
                Ciudad = perfil.Ciudad,
                Pais = perfil.Pais,
                ContactoEmergenciaNombre = perfil.ContactoEmergenciaNombre,
                ContactoEmergenciaTelefono = perfil.ContactoEmergenciaTelefono,
                ContactoEmergenciaRelacion = perfil.ContactoEmergenciaRelacion,
                NombrePadre = perfil.NombrePadre,
                NombreMadre = perfil.NombreMadre,
                NombreTutor = perfil.NombreTutor,
                Alergias = perfil.Alergias,
                CondicionesMedicas = perfil.CondicionesMedicas,
                Medicamentos = perfil.Medicamentos,
                Biografia = perfil.Biografia,
                Intereses = perfil.Intereses,
                Facebook = perfil.Facebook,
                Instagram = perfil.Instagram,
                Twitter = perfil.Twitter,
                CreadoEn = perfil.CreadoEn,
                ActualizadoEn = perfil.ActualizadoEn
            };
        }
    }
}
