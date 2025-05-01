using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Requests
{
    public class UserForCreateDTO
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Password { get; set; }
        public int RoleId { get; set; }
        public required string Phone { get; set; }
        public int? ApartmentId { get; set; }
    }
}
