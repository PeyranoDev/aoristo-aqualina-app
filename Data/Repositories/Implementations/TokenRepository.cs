using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<bool> AddNotificationTokenAsync(NotificationToken token)
        {
            try
            {
                await _context.NotificationTokens.AddAsync(token);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateNotificationTokenAsync(NotificationToken token)
        {
            try
            {
                _context.NotificationTokens.Update(token);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<NotificationToken?> GetNotificationTokenByTokenAsync(string token)
        {
            return await _context.NotificationTokens
                .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task<NotificationToken?> GetLatestTokenByUserIdAsync(int userId)
        {
            return await _context.NotificationTokens
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }
    }
}
