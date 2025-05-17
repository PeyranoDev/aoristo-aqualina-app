namespace Common.Models.Responses
{
    public class VehicleForResponseDTO
    {
        public int Id { get; set; }
        public string Plate { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public bool IsParked { get; set; }
        public bool IsActive { get; set; }
        public System.Collections.Generic.List<RequestForResponseDTO> Requests { get; set; }
    }
}