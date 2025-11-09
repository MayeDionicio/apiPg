using ApiPG.DTOs;
using ApiPG.Models;

namespace ApiPG.Services
{
    public interface ILevelService
    {
        Task<IEnumerable<NivelDto>> GetAllAsync();
        Task<NivelDto?> GetByIdAsync(int id);
        Task<NivelDto> CreateAsync(CrearNivelDto dto);
        Task<NivelDto?> UpdateAsync(int id, ActualizarNivelDto dto);
        Task<bool> SoftDeleteAsync(int id);

        Task<bool> AssignParticipantAsync(int levelId, int userId);
        Task<bool> RemoveParticipantAsync(int levelId, int userId);
        Task<bool> ChangeTutorAsync(int levelId, int? tutorId);
        Task<IEnumerable<ParticipanteDeNivelDto>> GetParticipantsAsync(int levelId);
        Task<IEnumerable<NivelDto>> GetLevelsByTutorAsync(int tutorId);
        Task<IEnumerable<UsuarioDto>> GetParticipantesElegiblesParaNivelAsync(int nivelId);
    }
}
