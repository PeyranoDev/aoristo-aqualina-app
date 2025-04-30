using Data.Entities;

namespace Data.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task<Role?> GetRoleByType(string type);
        Task<bool> RoleExistsAsync(int roleId);
    }
}