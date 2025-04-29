using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> ValidateAsync(string username, string hashedPassword);
        Task<User?> GetByIdAsync(int id);
        Task<User?> UpdateAsync(User user);

        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<User> CreateAsync(User user);
        Task<int> DeleteAsync(User user);
        IQueryable<User> GetQueryable();
    }
}