using Data.Entities;

namespace Data.Repositories.Interfaces
{
    public interface ITowerRepository
    {
        Task CreateAsync(Tower tower);
        Task DeleteAsync(Tower tower);
        IQueryable<Tower> GetAsQueryable();
        Task<Tower?> GetByIdAsync(int id);
        Task<Tower?> GetByNameAsync(string name);
        Task UpdateAsync(Tower tower);
    }
}