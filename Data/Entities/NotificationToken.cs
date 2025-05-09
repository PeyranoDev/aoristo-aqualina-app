using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class NotificationToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string? DeviceModel { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastSeen { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
