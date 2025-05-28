// Services/Main/Implementations/NotificationService.cs

using Common.Models.Requests;

namespace Services.Main.Interfaces
{
    public interface INotificationService
    {
        /// <summary>
        /// Envía una notificación al propietario del vehículo indicando que su vehículo está siendo preparado.
        /// </summary>
        /// <param name="vehicleId">ID del vehículo que se está preparando.</param>
        /// <param name="securityUserId">ID del usuario de seguridad que inicia la preparación.</param>
        /// <returns>Tarea asíncrona.</returns>
        /// <exception cref="Exception">Si no se encuentra el vehículo, el token o el usuario de seguridad.</exception>
        Task SendVehiclePreparationNotificationAsync(int vehicleId, int securityUserId);
        /// <summary>
        /// Envía una notificación a todos los usuarios de seguridad en servicio de que hay una nueva solicitud de vehículo.
        /// </summary>
        /// <param name="vehicleId">ID del vehículo solicitado.</param>
        /// <param name="userId">ID del usuario que hizo la solicitud.</param>
        /// <returns>Tarea asíncrona.</returns>
        Task SendVehicleRequestNotificationForSecurity(int vehicleId, int userId);
        /// <summary>
        /// Envía una notificación al propietario indicando que su vehículo está listo para ser retirado.
        /// </summary>
        /// <param name="vehicleId">ID del vehículo listo.</param>
        /// <returns>Tarea asíncrona.</returns>
        /// <exception cref="Exception">Si no se encuentra el token o el usuario asociado.</exception>
        Task SendVehicleReadyNotificationForUser(int vehicleId);
        /// <summary>
        /// Envía una notificación al propietario indicando que su vehículo está casi listo.
        /// </summary>
        /// <param name="vehicleId">ID del vehículo en preparación final.</param>
        /// <returns>Tarea asíncrona.</returns>
        /// <exception cref="Exception">Si no se encuentra el token o el usuario asociado.</exception>
        Task SendVehicleAlmostReadyNotificationForUser(int vehicleId);

        /// <summary>
        /// Envia una notificación al propietario del vehículo indicando que la solicitud ha sido cancelada.
        /// </summary>
        /// <param name="vehicleId"> ID del vehiculo que ha sido cancelado. </param>
        /// <returns> Tarea asincrona. </returns>
        /// <exception cref="Exception"> Si no se encuentra el token o el usuario asociado. </exception>
        Task SendVehicleCancelledNotificationForUser(int vehicleId);
    }
}