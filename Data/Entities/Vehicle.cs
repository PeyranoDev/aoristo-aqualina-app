using System;
using System.Collections.Generic;
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

        public List<VehicleRequest> Requests { get; set; }
    }
}

