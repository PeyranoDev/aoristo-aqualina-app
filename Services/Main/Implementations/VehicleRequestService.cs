using Common.Models.Requests;
using Data.Entities;
using Data.Enum;
using Data.Repositories.Interfaces;
using Services.Main.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Services.Main.Implementations
{
    public class VehicleRequestService : IVehicleRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;

        public VehicleRequestService(IRequestRepository requestRepository, IVehicleRepository vehicleRepository, INotificationService notificationService, IUserRepository userRepository)
        {
            _requestRepository = requestRepository;
            _vehicleRepository = vehicleRepository;
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public async Task<bool> CreateRequestAsync(int vehicleId, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
            {
                return false;
            }

            if (vehicle.OwnerId == userId)
            {
                throw new Exception("No tienes permiso para pedir este auto");
            }

            var request = new Request
            {
                VehicleId = vehicleId,
                RequestedById = userId,
                Vehicle = vehicle,
                RequestedBy = user,
                Status = VehicleRequestStatusEnum.Pending,
                RequestedAt = DateTime.UtcNow,

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
            var vehicle = await _vehicleRepository.GetByIdAsync(dto.VehicleId);
            if (vehicle == null)
            {
                return false;
            }

            var request = await _requestRepository.GetLatestByVehicleAsync(dto.VehicleId);

            if (request == null)
            {
                throw new Exception("No hay solicitudes pendientes para este vehículo.");
            }

            if (dto.VehicleRequestNewStatus == VehicleRequestStatusEnum.InPreparation)
            {
                request.Status = dto.VehicleRequestNewStatus;
                request.UpdatedAt = DateTime.UtcNow;

                var result = await _requestRepository.UpdateAsync(request);
                if (result)
                {
                    await _notificationService.SendVehiclePreparationNotificationAsync(dto.VehicleId, dto.SecurityId);
                }

                return result;
            }
            if (dto.VehicleRequestNewStatus == VehicleRequestStatusEnum.AlmostReady)
            {
                request.Status = dto.VehicleRequestNewStatus;
                request.UpdatedAt = DateTime.UtcNow;

                var result = await _requestRepository.UpdateAsync(request);
                if (result)
                {
                    await _notificationService.SendVehicleAlmostReadyNotificationForUser(dto.VehicleId);
                }
                else
                {
                    throw new Exception("La solicitud ya ha sido procesada.");
                }
                return result;
            }
            if (dto.VehicleRequestNewStatus == VehicleRequestStatusEnum.Ready)
            {
                request.Status = dto.VehicleRequestNewStatus;
                request.UpdatedAt = DateTime.UtcNow;
                request.CompletedAt = DateTime.UtcNow;

                var result = await _requestRepository.UpdateAsync(request);
                if (result)
                {
                    await _notificationService.SendVehicleReadyNotificationForUser(dto.VehicleId);
                }
                else
                {
                    throw new Exception("La solicitud ya ha sido procesada.");
                }
                return result;
            }
            if (dto.VehicleRequestNewStatus == VehicleRequestStatusEnum.Cancelled)
            {
                request.Status = dto.VehicleRequestNewStatus;
                request.UpdatedAt = DateTime.UtcNow;

                var result = await _requestRepository.UpdateAsync(request);
                if (result)
                {
                    await _notificationService.SendVehicleCancelledNotificationForUser(dto.VehicleId);
                }
                else
                {
                    throw new Exception("La solicitud ya ha sido procesada.");
                }
                return result;
            }
            if (dto.VehicleRequestNewStatus == VehicleRequestStatusEnum.Completed)
            {
                request.Status = dto.VehicleRequestNewStatus;
                request.UpdatedAt = DateTime.UtcNow;
                request.CompletedAt = DateTime.UtcNow;

                var result = await _requestRepository.UpdateAsync(request);
                if (result)
                {
                    await _notificationService.SendVehicleReadyNotificationForUser(dto.VehicleId);
                }
                else
                {
                    throw new Exception("La solicitud ya ha sido procesada.");
                }
                return result;
            }
            else
            {
                throw new Exception("Estado de solicitud no válido.");
            }
        }
    }
}
