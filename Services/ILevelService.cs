using ApiPG.DTOs;
using ApiPG.Models;

namespace ApiPG.Services
{
    public interface ILevelService
    {
        Task<IEnumerable<LevelDto>> GetAllAsync();
        Task<LevelDto?> GetByIdAsync(int id);
        Task<LevelDto> CreateAsync(CreateLevelDto dto);
        Task<LevelDto?> UpdateAsync(int id, UpdateLevelDto dto);
        Task<bool> SoftDeleteAsync(int id);

        Task<bool> AssignParticipantAsync(int levelId, int userId);
        Task<bool> RemoveParticipantAsync(int levelId, int userId);
        Task<bool> ChangeTutorAsync(int levelId, int? tutorId);
        Task<IEnumerable<LevelParticipantDto>> GetParticipantsAsync(int levelId);
        Task<IEnumerable<LevelDto>> GetLevelsByTutorAsync(int tutorId);
    }
}
