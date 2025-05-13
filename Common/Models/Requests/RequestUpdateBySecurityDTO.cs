using Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Requests
{
    public class RequestUpdateBySecurityDTO
    {
        public required int VehicleId { get; set; }
        public required VehicleRequestStatusEnum VehicleRequestNewStatus { get; set; }
        public required int SecurityId { get; set; }
    }
}
