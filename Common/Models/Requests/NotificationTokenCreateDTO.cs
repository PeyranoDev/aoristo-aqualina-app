using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Requests
{
    public class NotificationTokenCreateDTO
    {
        public string Token { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string? DeviceModel { get; set; }
    }
}
