
using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [ForeignKey(nameof(VehicleId))]
        public Vehicle Vehicle { get; set; }

        [Required]
        public VehicleRequestStatusEnum Status { get; set; }

        [Required]
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        [Required]
        public int RequestedById { get; set; }

        [ForeignKey(nameof(RequestedById))]
        public User RequestedBy { get; set; }

        public int? CompletedById { get; set; }

        [ForeignKey(nameof(CompletedById))]
        public User? CompletedBy { get; set; }

        [NotMapped]
        public bool IsCompletedBySecurity =>
            CompletedBy?.Role?.Type == UserRoleEnum.Security ||
            Status == VehicleRequestStatusEnum.Completed;
    }
}
