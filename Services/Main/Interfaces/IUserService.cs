using Common.Models;
using Common.Models.Requests;
using Common.Models.Responses.Common.Models.Responses;
using Data.Entities;
using System.Threading.Tasks;

namespace Services.Main.Interfaces
{
    public interface IUserService
    {
        Task<User?> ValidateAsync(CreedentialsDTO dto);
        Task<User?> UpdateUserAsync(UserForUpdateDTO dto, int userId);
        Task<User?> GetByIdAsync(int id);
        Task<int> DeleteUserAsync(int id);
        Task<PagedResponse<UserForResponse>> GetUsersPagedAsync(UserFilterParams filters, PaginationParams pagination);
    }
}