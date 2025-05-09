using Data.Entities;

namespace Data.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        Task<bool> AddNotificationTokenAsync(NotificationToken token);
        Task<NotificationToken?> GetNotificationTokenByTokenAsync(string token);
        Task<bool> UpdateNotificationTokenAsync(NotificationToken token);
    }
}