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
        public required string Username { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string PasswordHash { get; set; }
        public Role Role { get; set; }
        public required bool IsActive { get; set; }
        public required PhoneAttribute Phone { get; set; }
        public Apartment Apartment { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
        public ICollection<Vehicle>? OwnedCars { get; set; }
    }
}
