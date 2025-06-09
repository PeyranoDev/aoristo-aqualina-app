using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public required bool IsActive { get; set; }
        public required string Phone { get; set; }

        public int? ApartmentId { get; set; }
        public Apartment? Apartment { get; set; }

        [InverseProperty("RequestedBy")]
        public virtual ICollection<Request> RequestsMade { get; set; } = new List<Request>();

        [InverseProperty("CompletedBy")]
        public virtual ICollection<Request> RequestsCompleted { get; set; } = new List<Request>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Vehicle> OwnedCars { get; set; } = new List<Vehicle>();
        public ICollection<NotificationToken> NotificationTokens { get; set; } = new List<NotificationToken>();

        public ICollection<UserTower> UserTowers { get; set; } = new List<UserTower>();

        public bool IsOnDuty { get; set; } = false;

        public bool CanBookAmenity()
            => Role.Type == UserRoleEnum.User && IsActive;

        public ICollection<int> GetAssociatedTowerIds()
        {
            var towerIds = new HashSet<int>();

            if (ApartmentId.HasValue && Apartment != null)
            {
                towerIds.Add(Apartment.TowerId);
            }

            if (UserTowers != null)
            {
                foreach (var userTower in UserTowers)
                {
                    towerIds.Add(userTower.TowerId);
                }
            }

            return towerIds.ToList();
        }
    }
}