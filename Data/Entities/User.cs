using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required EmailAddressAttribute Email { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string PasswordHash { get; set; }
        public UserRoleEnum Role { get; set; }
        public required bool IsActive { get; set; }
        public required PhoneAttribute Phone { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
        public ICollection<UserCar>? OwnedCars { get; set; }
        public ICollection<UserCar>? SharedCars { get; set; }
    }
}
