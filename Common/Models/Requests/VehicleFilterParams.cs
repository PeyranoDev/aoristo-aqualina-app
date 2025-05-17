namespace Common.Models.Requests
{
    public class VehicleFilterParams
    {
        public string? Plate { get; set; }
        public string? Model { get; set; }
        public bool? IsActive { get; set; }
        public bool? HasRequests { get; set; }
        public bool IncludeRequests { get; set; } = false;
    }
}