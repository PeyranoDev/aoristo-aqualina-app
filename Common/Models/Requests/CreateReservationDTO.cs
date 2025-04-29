using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Requests
{
    public class CreateReservationDTO
    {
        public string Amenity { get; set; }
        public DateOnly Reservation_Date { get; set; }
        public TimeOnly Start_Time { get; set; }
        public TimeOnly End_Time { get; set; }
    }
}