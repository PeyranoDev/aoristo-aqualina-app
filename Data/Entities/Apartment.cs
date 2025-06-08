using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{

    public class Apartment
    {
        public int Id { get; set; }
        public string Identifier { get; set; } = string.Empty;
        public int TowerId { get; set; }
        public Tower Tower { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
