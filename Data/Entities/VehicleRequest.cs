using Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class VehicleRequest
    {
        public Guid Id { get; set; }

        // Foreign key
        public Guid VehicleId { get; set; }

        // Relación
        public Vehicle Vehicle { get; set; }

        // Estado de la solicitud
        public VehicleRequestStatusEnum Status { get; set; } 

        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
