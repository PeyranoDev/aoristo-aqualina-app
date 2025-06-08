using Common.Models.Requests;
using Data.Entities;
using Data.Enum;
using Data.Repositories.Interfaces;
using Services.Main.Interfaces;

namespace Services.Main.Implementations
{
    public class VehicleRequestService : IVehicleRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;

        public VehicleRequestService(
            IRequestRepository requestRepository,
            IVehicleRepository vehicleRepository,
            INotificationService notificationService,
            IUserRepository userRepository)
        {
            _requestRepository = requestRepository;
            _vehicleRepository = vehicleRepository;
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public async Task<bool> CreateRequestAsync(int vehicleId, int userId)
        {
            var vehicle = await ValidateVehicle(vehicleId);
            ValidateRequestPermission(vehicle, userId);

            var request = new Request
            {
                VehicleId = vehicleId,
                RequestedById = userId,
                Status = VehicleRequestStatusEnum.Pending,
                RequestedAt = DateTime.UtcNow
            };

            var result = await _requestRepository.AddAsync(request);
            if (result)
            {
                await _notificationService.SendVehicleRequestNotificationForSecurity(vehicleId, userId);
            }

            return result;
        }

        public async Task<bool> SecurityUpdateRequestAsync(RequestUpdateBySecurityDTO dto)
        {
            var (vehicle, request) = await ValidateRequest(dto.VehicleId);
            var securityUser = await ValidateSecurityUser(dto.SecurityId);

            UpdateRequestStatus(request, dto.VehicleRequestNewStatus, securityUser.Id);

            var result = await _requestRepository.UpdateAsync(request);
            if (!result)
            {
                throw new InvalidOperationException("Error al actualizar la solicitud");
            }

            await SendStatusNotification(dto.VehicleRequestNewStatus, vehicle, request.RequestedById, securityUser.Id);
            return true;
        }

        #region Private Methods
        private async Task<Vehicle> ValidateVehicle(int vehicleId)
        {
            return await _vehicleRepository.GetByIdAsync(vehicleId) ??
                throw new InvalidOperationException("Vehículo no encontrado");
        }

        private void ValidateRequestPermission(Vehicle vehicle, int userId)
        {
            if (vehicle.OwnerId == userId)
                throw new UnauthorizedAccessException("No puedes solicitar tu propio vehículo");
        }

        private async Task<(Vehicle vehicle, Request request)> ValidateRequest(int vehicleId)
        {
            var vehicle = await ValidateVehicle(vehicleId);
            var request = await _requestRepository.GetLatestByVehicleAsync(vehicleId) ??
                throw new InvalidOperationException("No hay solicitudes pendientes");

            return (vehicle, request);
        }

        private async Task<User> ValidateSecurityUser(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId) ??
                throw new InvalidOperationException("Usuario no encontrado");

            if (user.Role?.Type != UserRoleEnum.Security)
                throw new UnauthorizedAccessException("Solo personal de seguridad puede completar solicitudes");

            return user;
        }

        private void UpdateRequestStatus(Request request, VehicleRequestStatusEnum status, int securityId)
        {
            request.Status = status;
            request.UpdatedAt = DateTime.UtcNow;

            if (status == VehicleRequestStatusEnum.Completed || status == VehicleRequestStatusEnum.Ready)
            {
                request.CompletedById = securityId;
                request.CompletedAt = DateTime.UtcNow;
            }
        }

        private async Task SendStatusNotification(
            VehicleRequestStatusEnum status,
            Vehicle vehicle,
            int requestedById,
            int securityId)
        {
            switch (status)
            {
                case VehicleRequestStatusEnum.InPreparation:
                    await _notificationService.SendVehiclePreparationNotificationAsync(vehicle.Id, securityId);
                    break;
                case VehicleRequestStatusEnum.AlmostReady:
                    await _notificationService.SendVehicleAlmostReadyNotificationForUser(vehicle.Id);
                    break;
                case VehicleRequestStatusEnum.Ready:
                case VehicleRequestStatusEnum.Completed:
                    await _notificationService.SendVehicleReadyNotificationForUser(vehicle.Id);
                    break;
                case VehicleRequestStatusEnum.Cancelled:
                    await _notificationService.SendVehicleCancelledNotificationForUser(vehicle.Id);
                    break;
            }
        }
        #endregion
    }
}