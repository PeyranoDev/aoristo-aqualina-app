using Data.Entities;

namespace Data.Repositories.Interfaces
{
    public interface IInvitationRepository
    {
        Task<string> AddInvitationAsync(Invitation invitation);
        Task<Invitation?> GetByIdAsync(int id);
        Task<Invitation?> GetByTokenWithRoleAsync(string token);
        Task<bool> IsTokenValidAsync(string token);
        Task<int> UpdateInvitationAsync(Invitation invitation);
    }
}