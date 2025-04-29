using Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Reservation
    {
        public int id { get; set; }
        public Amenity Amenity { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public ReservationStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
