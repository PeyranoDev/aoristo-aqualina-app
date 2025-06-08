using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Plate { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public bool IsParked { get; set; }
        public bool IsActive { get; set; }
        public int OwnerId { get; set; }
        public ICollection<Request> Requests { get; set; } = new List<Request>();

        [NotMapped]
        public Request? ActiveRequest
            => Requests.FirstOrDefault(r =>
                r.Status == VehicleRequestStatusEnum.Pending ||
                r.Status == VehicleRequestStatusEnum.InPreparation ||
                r.Status == VehicleRequestStatusEnum.AlmostReady ||
                r.Status == VehicleRequestStatusEnum.Ready
            );
    }
}

