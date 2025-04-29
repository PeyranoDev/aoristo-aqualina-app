using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Requests
{
    public class UserFilterParams
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? RoleType { get; set; }
        public bool? IsActive { get; set; }
        public string? ApartmentIdentifier { get; set; }
    }
}
