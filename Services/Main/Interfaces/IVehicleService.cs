using Common.Models.Requests;
using Common.Models.Responses;
using Data.Entities;

namespace Services.Main.Interfaces
{
    public interface IVehicleService
    {
        Task<bool> DeleteVehicleAsync(int vehicleId);
        Task<Request?> GetLastActiveRequestAsync(int vehicleId);
        Task<PagedResponse<VehicleForResponseDTO>> GetVehiclesPagedAsync(VehicleFilterParams filters, PaginationParams pagination);
        Task<IList<Vehicle>> GetVehiclesPerUserIdAsync(int userId);
        Task<IList<Vehicle>> GetVehiclesPerUserIdWithLastRequestAsync(int userId);
        Task<IList<Vehicle>> GetVehiclesWithActiveRequestsAsync();
        Task<bool> HasActiveRequestAsync(int vehicleId);
        Task<Vehicle?> GetVehicleByIdAsync(int vehicleId);
    }
}