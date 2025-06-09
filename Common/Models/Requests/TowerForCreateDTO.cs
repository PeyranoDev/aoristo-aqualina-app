using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Requests
{
    public class TowerForCreateDTO
    {
        public string Name { get; set; }
        public string Adress { get; set; }
        public string? Description { get; set; }
    }
}
