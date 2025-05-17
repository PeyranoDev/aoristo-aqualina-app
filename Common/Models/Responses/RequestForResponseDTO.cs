namespace Common.Models.Responses
{
    public class RequestForResponseDTO
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public System.DateTime RequestedAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public System.DateTime? CompletedAt { get; set; }
        public string RequestedBy { get; set; }
    }
}