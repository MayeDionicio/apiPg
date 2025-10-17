using ApiPG.DTOs;
using ApiPG.Data;
using ApiPG.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPG.Services
{
    public class LevelService : ILevelService
    {
        private readonly ApiPGContext _db;

        public LevelService(ApiPGContext db)
        {
            _db = db;
        }

        public async Task<NivelDto> CreateAsync(CrearNivelDto dto)
        {
            var level = new Nivel
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                IdVoluntario = dto.VoluntarioId,
                CreadoEn = DateTime.UtcNow,
                EstaActivo = true
            };

            _db.Niveles.Add(level);
            await _db.SaveChangesAsync();

            return await GetByIdInternalAsync(level.Id) ?? throw new InvalidOperationException("Failed to load created level");
        }

        public async Task<IEnumerable<NivelDto>> GetAllAsync()
        {
            var levels = await _db.Niveles
                .Where(l => l.EstaActivo)
                .Include(l => l.ParticipantesDelNivel)
                .ThenInclude(lp => lp.Usuario)
                .ToListAsync();

            return levels.Select(l => new NivelDto
            {
                Id = l.Id,
                Nombre = l.Nombre,
                Descripcion = l.Descripcion,
                VoluntarioId = l.IdVoluntario,
                NombreVoluntario = l.Voluntario != null ? $"{l.Voluntario.PrimerNombre} {l.Voluntario.Apellido}" : null,
                CreadoEn = l.CreadoEn,
                EstaActivo = l.EstaActivo,
                CantidadParticipantes = l.ParticipantesDelNivel.Count(lp => lp.EstaActivo)
            });
        }

        public async Task<NivelDto?> GetByIdAsync(int id)
        {
            return await GetByIdInternalAsync(id);
        }

        private async Task<NivelDto?> GetByIdInternalAsync(int id)
        {
            var l = await _db.Niveles
                .Include(x => x.ParticipantesDelNivel)
                .ThenInclude(lp => lp.Usuario)
                .Include(x => x.Voluntario)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (l == null) return null;

            return new NivelDto
            {
                Id = l.Id,
                Nombre = l.Nombre,
                Descripcion = l.Descripcion,
                VoluntarioId = l.IdVoluntario,
                NombreVoluntario = l.Voluntario != null ? $"{l.Voluntario.PrimerNombre} {l.Voluntario.Apellido}" : null,
                CreadoEn = l.CreadoEn,
                EstaActivo = l.EstaActivo,
                CantidadParticipantes = l.ParticipantesDelNivel.Count(lp => lp.EstaActivo)
            };
        }

        public async Task<NivelDto?> UpdateAsync(int id, ActualizarNivelDto dto)
        {
            var l = await _db.Niveles.FindAsync(id);
            if (l == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Nombre)) l.Nombre = dto.Nombre;
            if (dto.EstaActivo.HasValue) l.EstaActivo = dto.EstaActivo.Value;
            if (dto.VoluntarioId.HasValue)
            {
                // Change tutor: just set the TutorId (historical assignments can be kept elsewhere if needed)
                l.IdVoluntario = dto.VoluntarioId;
            }
            if (!string.IsNullOrWhiteSpace(dto.Descripcion)) l.Descripcion = dto.Descripcion;

            await _db.SaveChangesAsync();
            return await GetByIdInternalAsync(id);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var l = await _db.Niveles.FindAsync(id);
            if (l == null) return false;
            l.EstaActivo = false;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignParticipantAsync(int levelId, int userId)
        {
            // If there is an existing inactive association, reactivate it; otherwise create a new one
            var lp = await _db.NivelesDeParticipantes.FirstOrDefaultAsync(x => x.IdNivel == levelId && x.IdUsuario == userId);
            if (lp != null)
            {
                if (lp.EstaActivo) return true;
                lp.EstaActivo = true;
                lp.AsignadoEn = DateTime.UtcNow;
            }
            else
            {
                lp = new NivelDeParticipante
                {
                    IdNivel = levelId,
                    IdUsuario = userId,
                    AsignadoEn = DateTime.UtcNow,
                    EstaActivo = true
                };
                _db.NivelesDeParticipantes.Add(lp);
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveParticipantAsync(int levelId, int userId)
        {
            var lp = await _db.NivelesDeParticipantes.FirstOrDefaultAsync(x => x.IdNivel == levelId && x.IdUsuario == userId && x.EstaActivo);
            if (lp == null) return false;
            // Soft remove
            lp.EstaActivo = false;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeTutorAsync(int levelId, int? tutorId)
        {
            var l = await _db.Niveles.FindAsync(levelId);
            if (l == null) return false;
            l.IdVoluntario = tutorId;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ParticipanteDeNivelDto>> GetParticipantsAsync(int levelId)
        {
            var list = await _db.NivelesDeParticipantes
                .Where(lp => lp.IdNivel == levelId && lp.EstaActivo)
                .Include(lp => lp.Usuario)
                .Select(lp => new ParticipanteDeNivelDto
                {
                    UsuarioId = lp.IdUsuario,
                    NombreUsuario = lp.Usuario.NombreCompleto,
                    AsignadoEn = lp.AsignadoEn,
                    EstaActivo = lp.EstaActivo
                })
                .ToListAsync();

            return list;
        }

        public async Task<IEnumerable<NivelDto>> GetLevelsByTutorAsync(int tutorId)
        {
            var levels = await _db.Niveles
                .Where(l => l.EstaActivo && l.IdVoluntario == tutorId)
                .Include(l => l.ParticipantesDelNivel)
                .ThenInclude(lp => lp.Usuario)
                .Include(l => l.Voluntario)
                .ToListAsync();

            return levels.Select(l => new NivelDto
            {
                Id = l.Id,
                Nombre = l.Nombre,
                Descripcion = l.Descripcion,
                VoluntarioId = l.IdVoluntario,
                NombreVoluntario = l.Voluntario != null ? $"{l.Voluntario.PrimerNombre} {l.Voluntario.Apellido}" : null,
                CreadoEn = l.CreadoEn,
                EstaActivo = l.EstaActivo,
                CantidadParticipantes = l.ParticipantesDelNivel.Count(lp => lp.EstaActivo)
            });
        }
    }
}
