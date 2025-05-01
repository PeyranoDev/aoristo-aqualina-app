using Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Responses
{
    public class RoleResponseDto
    {
        public int Id { get; set; }
        public UserRoleEnum Type { get; set; }
    }
}
