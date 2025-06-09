using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class UserTower
    {
        public int UserId { get; set; }
        public int TowerId { get; set; }

        public User User { get; set; } = null!;
        public Tower Tower { get; set; } = null!; 
    }
}