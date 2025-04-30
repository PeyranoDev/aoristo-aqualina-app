using Common.Models;
using Common.Models.Requests;
using Common.Models.Responses.Common.Models.Responses;

namespace Services.Main.Interfaces
{
    public interface IUserService
    {
        Task<UserForResponse> CreateUserAsync(UserForCreateDTO dto);
        Task<User> CreateUserAsyncWithInvitation(RegisterWithTokenDto dto, int roleId, int? apartmentId);
        Task<int> DeleteUserAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<UserForResponse?> GetByIdAsync(int id);
        Task<PagedResponse<UserForResponse>> GetUsersPagedAsync(UserFilterParams filters, PaginationParams pagination);
        Task<User?> UpdateUserAsync(UserForUpdateDTO dto, int userId);
        Task<bool> UsernameExistsAsync(string username);
        Task<User?> ValidateAsync(CredentialsDTO dto);
    }
}