using Common.Models.Requests;

namespace Services.Main.Interfaces
{
    public interface ITokenService
    {
        Task<bool> AddNotificationTokenAsync(NotificationTokenCreateDTO dto, int userId);
    }
}