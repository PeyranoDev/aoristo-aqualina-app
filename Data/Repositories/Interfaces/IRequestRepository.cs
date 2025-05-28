using Data.Entities;

namespace Data.Repositories.Interfaces
{
    public interface IRequestRepository
    {
        Task<bool> AddAsync(Request request);
        Task<List<Request>> GetAllAsync();
        Task<bool> UpdateAsync(Request request);
        Task<Request?> GetLatestByVehicleAsync(int vehicleId);
    }
}