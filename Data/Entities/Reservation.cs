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
        public DateTime Reservation_Date { get; set; }
        public TimeOnly Start_Time { get; set; }
        public TimeOnly End_Time { get; set; }
        public ReservationStatusEnum Status { get; set; }
        public DateTime Created_At { get; set; }
    }
}
