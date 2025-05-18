using Common.Models.Requests;
using Common.Models.Responses.Common.Models.Responses;
using Common.Models.Responses;
using Data.Entities;

namespace Services.Main.Interfaces
{
    public interface IVehicleService
    {
        Task<bool> DeleteVehicleAsync(int vehicleId);
        Task<IList<Vehicle>> GetVehiclesPerUserIdAsync(int userId);
        Task<PagedResponse<VehicleForResponseDTO>> GetVehiclesPagedAsync(VehicleFilterParams filters, PaginationParams pagination);
        Task<bool> HasActiveRequestAsync(int vehicleId);
        Task<Request?> GetLastActiveRequestAsync(int vehicleId);
        Task<IList<Vehicle>> GetVehiclesPerUserIdWithLastRequestAsync(int userId);
        Task<IList<Vehicle>> GetVehiclesWithActiveRequestsAsync();
    }
}