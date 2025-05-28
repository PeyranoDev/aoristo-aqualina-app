namespace Common.Models.Requests
{
    public class NotificationVehiclePreparingDTO
    {
        public int VehicleId { get; set; }
        public string VehicleModel { get; set; } = string.Empty;
    }
}