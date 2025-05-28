using Common.Models.Requests;

namespace Services.Main.Interfaces
{
    public interface IVehicleRequestService
    {
        Task<bool> CreateRequestAsync(int vehicleId, int userId);
        Task<bool> SecurityUpdateRequestAsync(RequestUpdateBySecurityDTO dto);
    }
}