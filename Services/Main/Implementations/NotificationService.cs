using Data.Entities;
using Data.Repositories.Interfaces;
using FirebaseAdmin.Messaging;
using Services.Main.Interfaces;
using Services.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Main.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly FirebaseProvider _firebaseProvider;

        public NotificationService(
            IUserRepository userRepository,
            ITokenRepository tokenRepository,
            IVehicleRepository vehicleRepository,
            FirebaseProvider firebaseProvider)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _vehicleRepository = vehicleRepository;
            _firebaseProvider = firebaseProvider;
        }

        public async Task SendVehiclePreparationNotificationAsync(int vehicleId, int securityUserId)
        {
            var (vehicle, owner, token) = await GetValidatedVehicleOwnerAndToken(vehicleId);
            var securityUser = await GetUserWithValidation(securityUserId);

            await SendNotification(new NotificationRequest
            {
                UserId = vehicle.OwnerId,
                Title = $"{owner.Name}, tu {vehicle.Model} está siendo preparado!",
                Body = $"Está siendo preparado por {securityUser.Name}. En unos minutos te avisaremos cuando esté casi listo!",
                Type = "vehicle_preparing",
                Data = new Dictionary<string, string> { { "vehicleModel", vehicle.Model } }
            });
        }

        public async Task SendVehicleRequestNotificationForSecurity(int vehicleId, int userId)
        {
            var securityUsers = await _userRepository.GetAllOnDutySecurityAsync();
            var tasks = securityUsers.Select(user =>
                SendSecurityNotification(user, vehicleId, userId)
            );

            await Task.WhenAll(tasks);
        }

        public async Task SendVehicleReadyNotificationForUser(int vehicleId)
        {
            var (vehicle, owner, token) = await GetValidatedVehicleOwnerAndToken(vehicleId);
            await SendNotification(new NotificationRequest
            {
                UserId = vehicle.OwnerId,
                Title = $"{owner.Name}, ¡tu vehículo está listo!",
                Body = $"Tu {vehicle.Model} está listo para ser recogido.",
                Type = "vehicle_ready"
            });
        }

        public async Task SendVehicleAlmostReadyNotificationForUser(int vehicleId)
        {
            var (vehicle, owner, token) = await GetValidatedVehicleOwnerAndToken(vehicleId);
            await SendNotification(new NotificationRequest
            {
                UserId = vehicle.OwnerId,
                Title = $"{owner.Name}, tu {vehicle.Model} está casi listo!",
                Body = "Por favor ve dirigiéndote al entrepiso para retirarlo.",
                Type = "vehicle_almost_ready"
            });
        }

        public async Task SendVehicleCancelledNotificationForUser(int vehicleId)
        {
            var (vehicle, owner, token) = await GetValidatedVehicleOwnerAndToken(vehicleId);
            await SendNotification(new NotificationRequest
            {
                UserId = vehicle.OwnerId,
                Title = $"{owner.Name}, tu {vehicle.Model} ha sido cancelado!",
                Body = "¡Haz click para ver más información!",
                Type = "vehicle_cancelled"
            });
        }

        #region Private Methods
        private async Task SendNotification(NotificationRequest request)
        {
            var firebaseApp = await _firebaseProvider.GetAppAsync();
            if (firebaseApp == null)
                throw new InvalidOperationException("Firebase app is not initialized.");

            var token = await GetValidNotificationToken(request.UserId);
            if (token == null) return;

            var message = new Message
            {
                Token = token.Token,
                Notification = new Notification
                {
                    Title = request.Title,
                    Body = request.Body
                },
                Data = new Dictionary<string, string>
                {
                    { "type", request.Type },
                    { "vehicleId", request.UserId.ToString() }
                }
            };

            if (request.Data != null)
            {
                var mutableData = new Dictionary<string, string>(message.Data);
                foreach (var item in request.Data)
                    mutableData.Add(item.Key, item.Value);

                message.Data = mutableData;
            }

            await FirebaseMessaging.GetMessaging(firebaseApp).SendAsync(message);
            await UpdateTokenLastUsed(token.Id);
        }

        private async Task SendSecurityNotification(User user, int vehicleId, int requestedById)
        {
            var token = await GetValidNotificationToken(user.Id);
            if (token == null) return;

            var message = new Message
            {
                Token = token.Token,
                Notification = new Notification
                {
                    Title = $"Hola {user.Name}, ¡nueva solicitud de vehículo!",
                    Body = "Revisa la aplicación para más detalles."
                },
                Data = new Dictionary<string, string>
                {
                    { "type", "security_vehicle_request" },
                    { "vehicleId", vehicleId.ToString() },
                    { "requestedById", requestedById.ToString() }
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
            await UpdateTokenLastUsed(token.Id);
        }

        private async Task<NotificationToken?> GetValidNotificationToken(int userId)
        {
            var token = await _tokenRepository.GetLatestByUserIdAsync(userId);
            return (token != null && !string.IsNullOrEmpty(token.Token)) ? token : null;
        }

        private async Task UpdateTokenLastUsed(int tokenId)
        {
            await _tokenRepository.UpdateLastUsedAsync(tokenId);
        }

        private async Task<(Vehicle vehicle, User owner, NotificationToken token)> GetValidatedVehicleOwnerAndToken(int vehicleId)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId) ??
                throw new InvalidOperationException("Vehículo no encontrado");

            var owner = await _userRepository.GetUserWithNotificationTokenAsync(vehicle.OwnerId) ??
                throw new InvalidOperationException("Dueño del vehículo no encontrado");

            var token = owner.NotificationTokens
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefault(t => !string.IsNullOrEmpty(t.Token)) ??
                throw new InvalidOperationException("Usuario no tiene token de notificación válido");

            return (vehicle, owner, token);
        }

        private async Task<User> GetUserWithValidation(int userId)
        {
            return await _userRepository.GetByIdAsync(userId) ??
                throw new InvalidOperationException("Usuario no encontrado");
        }

        private class NotificationRequest
        {
            public int UserId { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }
            public string Type { get; set; }
            public Dictionary<string, string>? Data { get; set; }
        }
        #endregion
    }
}