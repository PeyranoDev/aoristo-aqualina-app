
using Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Request
    {
        public int Id { get; set; }

        public int VehicleId { get; set; }

        public Vehicle Vehicle { get; set; }

        public VehicleRequestStatusEnum Status { get; set; } 

        public DateTime RequestedAt { get; set; }
        public DateTime UpdatedAt { get; set; } 
        public DateTime? CompletedAt { get; set; }

        public int RequestedById { get; set; }
        public User RequestedBy { get; set; }
    }
}
