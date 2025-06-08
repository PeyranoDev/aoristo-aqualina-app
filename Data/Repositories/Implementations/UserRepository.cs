using Data.Entities;
using Data.Enum;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Data.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AqualinaAPIContext _context;

        public UserRepository(AqualinaAPIContext context)
        {
            _context = context;
        }

        public async Task<User?> ValidateAsync(string username, string hashedPassword)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hashedPassword);
        }
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Apartment)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> UpdateAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null) return null;

            _context.Entry(existingUser).CurrentValues.SetValues(user);

            if (user.Apartment != null)
            {
                existingUser.Apartment = user.Apartment;
            }

            await _context.SaveChangesAsync();
            return existingUser;
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<int> DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            return await _context.SaveChangesAsync();
        }
        public IQueryable<User> GetQueryable()
        {
            return _context.Users
                .Include(u => u.Role)
                .Include(u => u.Apartment)
                .AsQueryable();
        }

        public async Task<IList<User>> GetAllSecurityAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Apartment)
                .Where(u => u.Role.Type == UserRoleEnum.Security)
                .ToListAsync();
        }

        public async Task<List<User>> GetAllOnDutySecurityAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.Type == UserRoleEnum.Security && u.IsOnDuty == true)
                .ToListAsync();
        }
        public async Task<User> GetUserWithNotificationTokenAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.NotificationTokens)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}