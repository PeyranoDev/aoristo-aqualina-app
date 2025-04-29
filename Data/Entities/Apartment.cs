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
        public ICollection<User> Users { get; set; }

    }
}
