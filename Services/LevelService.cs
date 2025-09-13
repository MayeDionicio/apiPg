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

        public async Task<LevelDto> CreateAsync(CreateLevelDto dto)
        {
            var level = new Level
            {
                Name = dto.Name,
                Description = dto.Description,
                TutorId = dto.TutorId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _db.Levels.Add(level);
            await _db.SaveChangesAsync();

            return await GetByIdInternalAsync(level.Id) ?? throw new InvalidOperationException("Failed to load created level");
        }

        public async Task<IEnumerable<LevelDto>> GetAllAsync()
        {
            var levels = await _db.Levels
                .Where(l => l.IsActive)
                .Include(l => l.LevelParticipants)
                .ThenInclude(lp => lp.User)
                .ToListAsync();

            return levels.Select(l => new LevelDto
            {
                Id = l.Id,
                Name = l.Name,
                Description = l.Description,
                TutorId = l.TutorId,
                TutorName = l.Tutor != null ? $"{l.Tutor.FirstName} {l.Tutor.LastName}" : null,
                CreatedAt = l.CreatedAt,
                IsActive = l.IsActive,
                ParticipantsCount = l.LevelParticipants.Count(lp => lp.IsActive)
            });
        }

        public async Task<LevelDto?> GetByIdAsync(int id)
        {
            return await GetByIdInternalAsync(id);
        }

        private async Task<LevelDto?> GetByIdInternalAsync(int id)
        {
            var l = await _db.Levels
                .Include(x => x.LevelParticipants)
                .ThenInclude(lp => lp.User)
                .Include(x => x.Tutor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (l == null) return null;

            return new LevelDto
            {
                Id = l.Id,
                Name = l.Name,
                Description = l.Description,
                TutorId = l.TutorId,
                TutorName = l.Tutor != null ? $"{l.Tutor.FirstName} {l.Tutor.LastName}" : null,
                CreatedAt = l.CreatedAt,
                IsActive = l.IsActive,
                ParticipantsCount = l.LevelParticipants.Count(lp => lp.IsActive)
            };
        }

        public async Task<LevelDto?> UpdateAsync(int id, UpdateLevelDto dto)
        {
            var l = await _db.Levels.FindAsync(id);
            if (l == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Name)) l.Name = dto.Name;
            if (dto.IsActive.HasValue) l.IsActive = dto.IsActive.Value;
            if (dto.TutorId.HasValue)
            {
                // Change tutor: just set the TutorId (historical assignments can be kept elsewhere if needed)
                l.TutorId = dto.TutorId;
            }
            if (!string.IsNullOrWhiteSpace(dto.Description)) l.Description = dto.Description;

            await _db.SaveChangesAsync();
            return await GetByIdInternalAsync(id);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var l = await _db.Levels.FindAsync(id);
            if (l == null) return false;
            l.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignParticipantAsync(int levelId, int userId)
        {
            // If there is an existing inactive association, reactivate it; otherwise create a new one
            var lp = await _db.LevelParticipants.FirstOrDefaultAsync(x => x.LevelId == levelId && x.UserId == userId);
            if (lp != null)
            {
                if (lp.IsActive) return true;
                lp.IsActive = true;
                lp.AssignedAt = DateTime.UtcNow;
            }
            else
            {
                lp = new LevelParticipant
                {
                    LevelId = levelId,
                    UserId = userId,
                    AssignedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _db.LevelParticipants.Add(lp);
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveParticipantAsync(int levelId, int userId)
        {
            var lp = await _db.LevelParticipants.FirstOrDefaultAsync(x => x.LevelId == levelId && x.UserId == userId && x.IsActive);
            if (lp == null) return false;
            // Soft remove
            lp.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeTutorAsync(int levelId, int? tutorId)
        {
            var l = await _db.Levels.FindAsync(levelId);
            if (l == null) return false;
            l.TutorId = tutorId;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LevelParticipantDto>> GetParticipantsAsync(int levelId)
        {
            var list = await _db.LevelParticipants
                .Where(lp => lp.LevelId == levelId && lp.IsActive)
                .Include(lp => lp.User)
                .Select(lp => new LevelParticipantDto
                {
                    UserId = lp.UserId,
                    UserName = lp.User.FullName,
                    AssignedAt = lp.AssignedAt,
                    IsActive = lp.IsActive
                })
                .ToListAsync();

            return list;
        }

        public async Task<IEnumerable<LevelDto>> GetLevelsByTutorAsync(int tutorId)
        {
            var levels = await _db.Levels
                .Where(l => l.IsActive && l.TutorId == tutorId)
                .Include(l => l.LevelParticipants)
                .ThenInclude(lp => lp.User)
                .Include(l => l.Tutor)
                .ToListAsync();

            return levels.Select(l => new LevelDto
            {
                Id = l.Id,
                Name = l.Name,
                Description = l.Description,
                TutorId = l.TutorId,
                TutorName = l.Tutor != null ? $"{l.Tutor.FirstName} {l.Tutor.LastName}" : null,
                CreatedAt = l.CreatedAt,
                IsActive = l.IsActive,
                ParticipantsCount = l.LevelParticipants.Count(lp => lp.IsActive)
            });
        }
    }
}
