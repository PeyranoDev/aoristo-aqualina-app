using Common.Models.Responses;
using Data.Entities;

namespace Services.Main.Interfaces
{
    public interface IInvitationService
    {
        Task<Invitation?> GetInvitationAsync(string token);
        Task<string> CreateInvitationAsync(CreateInvitationDto dto, int userId);
        Task<bool> IsTokenValidAsync(string token);
        Task<int> UpdateInvitationAsync(Invitation invitation);
    }
}