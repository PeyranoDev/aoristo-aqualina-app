using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Requests
{
    public class VehicleForUpdateDTO
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public bool IsParked { get; set; }
    }
}
