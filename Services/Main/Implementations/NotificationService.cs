using Data.Entities;
using Data.Repositories.Interfaces;
using FirebaseAdmin.Messaging;
using Services.Main.Interfaces;
using System.Runtime.InteropServices.Marshalling;

namespace Services.Main.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public NotificationService(
            IUserRepository userRepository,
            ITokenRepository TokenRepository,
            IVehicleRepository vehicleRepository)
        {
            _userRepository = userRepository;
            _tokenRepository = TokenRepository;
            _vehicleRepository = vehicleRepository;
        }

        /// <summary>
        /// Valida que el <see cref="NotificationToken"/> y su usuario asociado existan.
        /// </summary>
        /// <param name="token">Entidad de token de notificación a validar.</param>
        /// <returns>Siempre devuelve <c>true</c> si no arroja excepción.</returns>
        /// <exception cref="Exception">Si <paramref name="token"/> es <c>null</c> o su <see cref="NotificationToken.User"/> es <c>null</c>.</exception>
        private bool ValidateToken(NotificationToken token)
        {
            if (token == null || token.User == null)
                throw new Exception("El usuario del vehículo no tiene token de notificación registrado.");

            return true;
        }

        /// <summary>
        /// Verifica que el <see cref="Vehicle"/> exista y que el <see cref="NotificationToken"/> esté vinculado a un usuario.
        /// </summary>
        /// <param name="vehicle">Entidad de vehículo a validar.</param>
        /// <param name="token">Entidad de token de notificación a validar.</param>
        /// <returns>Siempre devuelve <c>true</c> si pasa las validaciones.</returns>
        /// <exception cref="Exception">
        /// Si <paramref name="vehicle"/> es <c>null</c>, o si <paramref name="token"/> es <c>null</c> o su <see cref="NotificationToken.User"/> es <c>null</c>.
        /// </exception>
        private bool Validations(Vehicle vehicle, NotificationToken token)
        {
            if (vehicle == null)
                throw new Exception("Vehículo no encontrado.");

            if (token == null || token.User == null)
                throw new Exception("El usuario del vehículo no tiene token de notificación registrado.");

            return true;

        }
        public async Task SendVehiclePreparationNotificationAsync(int vehicleId, int securityUserId)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

            var userId = vehicle.OwnerId;
            var vehicleModel = vehicle.Model;

            var tokenEntity = await _tokenRepository.GetLatestTokenByUserIdAsync(userId);

            try
            {
                Validations(vehicle, tokenEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en las validaciones: " + ex.Message);
            }

            var userName = tokenEntity.User.Name;
            var securityUser = await _userRepository.GetByIdAsync(securityUserId);

            var securityName = securityUser.Name;

            var title = $"{userName}, tu {vehicleModel} está siendo preparado!";
            var body = $"Hola {userName}! Tu {vehicleModel} está siendo preparado por {securityName}. En unos minutos te avisaremos cuando esté casi listo!";

            var message = new Message
            {
                Token = tokenEntity.Token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = new Dictionary<string, string>
                {
                    { "type", "vehicle_preparing" },
                    { "vehicleModel", vehicleModel }
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }

        public async Task SendVehicleRequestNotificationForSecurity(int vehicleId, int userId)
        {
            var securityUsers = await _userRepository.GetAllOnDutySecurityAsync();

            var messages = new List<Message>();

            foreach (var user in securityUsers)
            {
                var tokenEntity = await _tokenRepository.GetLatestTokenByUserIdAsync(user.Id);

                if (tokenEntity == null || string.IsNullOrEmpty(tokenEntity.Token))
                    continue;

                var title = $"Hola {user.Name}, ¡tienes una nueva solicitud de vehículo!";
                var body = $"Revisa la aplicación para ver más detalles de la solicitud.";

                var message = new Message
                {
                    Token = tokenEntity.Token,
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Data = new Dictionary<string, string>
            {
                { "type", "security_vehicle_request" },
                { "vehicleId", vehicleId.ToString() },
                { "requestedById", userId.ToString() }
            }
                };

                messages.Add(message);
            }

          
            foreach (var message in messages)
            {
                try
                {
                    await FirebaseMessaging.DefaultInstance.SendAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al enviar notificación a {message.Token}: {ex.Message}");
                }
            }
        }

        public async Task SendVehicleReadyNotificationForUser(int vehicleId)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

            var userId = vehicle.OwnerId;
            var vehicleModel = vehicle.Model;

            var tokenEntity = await _tokenRepository.GetLatestTokenByUserIdAsync(userId);

            try
            {
                ValidateToken(tokenEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en las validaciones: " + ex.Message);
            }

            var userName = tokenEntity.User.Name;

            var title = $"{userName}, tu {vehicleModel} está listo!";
            var body = $"Hola {userName}! Tu {vehicleModel} está listo para ser recogido. Por favor revisa la aplicación para más detalles.";

            var message = new Message
            {
                Token = tokenEntity.Token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = new Dictionary<string, string>
                {
                    { "type", "vehicle_ready" },
                    { "vehicleModel", vehicleModel }
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);

        }
        public async Task SendVehicleAlmostReadyNotificationForUser(int vehicleId)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

            var userId = vehicle.OwnerId;
            var vehicleModel = vehicle.Model;

            var tokenEntity = await _tokenRepository.GetLatestTokenByUserIdAsync(userId);
            try
            {
                ValidateToken(tokenEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en las validaciones: " + ex.Message);
            }

            var userName = tokenEntity.User.Name;

            var title = $"{userName}, tu {vehicleModel} está casi listo!";
            var body = $"Hola {userName}! Tu {vehicleModel} está casi listo para ser recogido. Por favor ve dirigiendote al entrepiso para retirarlo.";

            var message = new Message
            {
                Token = tokenEntity.Token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = new Dictionary<string, string>
                {
                    { "type", "vehicle_almost_ready" },
                    { "vehicleModel", vehicleModel }
                }
            };
            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
        public async Task SendVehicleCancelledNotificationForUser(int vehicleId)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

            var userId = vehicle.OwnerId;
            var vehicleModel = vehicle.Model;

            var tokenEntity = await _tokenRepository.GetLatestTokenByUserIdAsync(userId);
            try
            {
                ValidateToken(tokenEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en las validaciones: " + ex.Message);
            }

            var userName = tokenEntity.User.Name;

            var title = $"{userName}, tu {vehicleModel} ha sido cancelado!";
            var body = $"Hola {userName}! Tu pedido para el vehiculo {vehicleModel} ha sido cancelado, ¡Haz click para ver maas informacion! .";

            var message = new Message
            {
                Token = tokenEntity.Token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = new Dictionary<string, string>
                {
                    { "type", "vehicle_cancelled" },
                    { "vehicleModel", vehicleModel }
                }
            };
            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }
}
   
