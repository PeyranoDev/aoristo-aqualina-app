using Data.Entities;

namespace Data.Repositories.Interfaces
{
    public interface IVehicleRepository
    {
        Task<bool> AddAsync(Vehicle vehicle);
        Task<Vehicle?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Vehicle vehicle);
        IQueryable<Vehicle> GetAll();
        Task<IList<Vehicle>> GetVehiclesPerUserIdAsync(int userId);
    }
}