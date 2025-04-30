using Common.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class UserForResponse
    {
        public int Id { get; set; } 
        public string Email { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Role { get; set; } 
        public int RoleId { get; set; }  
        public string Phone { get; set; }
        public string Apartment { get; set; } 
        public int? ApartmentId { get; set; } 
        public bool IsActive { get; set; }
        public string RoleType { get; set; }
        public ApartmentInfoDTO ApartmentInfo { get; set; }
    }
}
