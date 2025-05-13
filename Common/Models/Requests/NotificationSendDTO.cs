using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Requests
{
    public class NotificationSendDTO
    {
        public required string Title { get; set; }
        public required string Body { get; set; }
    }
}
