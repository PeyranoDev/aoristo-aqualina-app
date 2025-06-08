using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories.Implementations
{
    public class TokenRepository : ITokenRepository
    {
        private readonly AqualinaAPIContext _context;

        public TokenRepository(AqualinaAPIContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(NotificationToken token)
        {
            try
            {
                token.CreatedAt = DateTime.UtcNow;
                token.UpdatedAt = DateTime.UtcNow;

                await _context.NotificationTokens.AddAsync(token);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(NotificationToken token)
        {
            try
            {
                token.UpdatedAt = DateTime.UtcNow;
                _context.NotificationTokens.Update(token);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateLastUsedAsync(int tokenId)
        {
            try
            {
                var token = await _context.NotificationTokens.FindAsync(tokenId);
                if (token == null) return false;

                token.LastSeen = DateTime.UtcNow;
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<NotificationToken?> GetByTokenAsync(string tokenString)
        {
            return await _context.NotificationTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Token == tokenString);
        }

        public async Task<NotificationToken?> GetLatestByUserIdAsync(int userId)
        {
            return await _context.NotificationTokens
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteExpiredTokensAsync(TimeSpan expirationTime)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.Subtract(expirationTime);
                var expiredTokens = await _context.NotificationTokens
                    .Where(t => t.LastSeen < cutoffDate ||
                               (t.LastSeen == null && t.CreatedAt < cutoffDate))
                    .ToListAsync();

                _context.NotificationTokens.RemoveRange(expiredTokens);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
    }
}