using Data.Entities;

namespace Data.Repositories.Interfaces
{
    public interface IVehicleRepository
    {
        Task<bool> AddAsync(Vehicle vehicle);
        Task<List<Vehicle>> GetAllWithoutRequestsAsync();
        Task<Vehicle?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Vehicle vehicle);
    }
}