using Common.Models;
using Data.Entities;

namespace Services.Main.Interfaces
{
    public interface IUserService
    {
        Task<User?> ValidateAsync(CreedentialsDTO dto);
        Task<User?> UpdateUserAsync(UserForUpdateDTO dto, int userId);
        Task<User?> GetByIdAsync(int id);
        Task<int> DeleteUserAsync(int id);
    }
}