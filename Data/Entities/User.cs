using Data.Entities;
using Data.Enum;
using System.ComponentModel.DataAnnotations.Schema;

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
    public virtual ICollection<Request> RequestsMade { get; set; }

    [InverseProperty("CompletedBy")]
    public virtual ICollection<Request> RequestsCompleted { get; set; }
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Vehicle> OwnedCars { get; set; } = new List<Vehicle>();
    public ICollection<NotificationToken> NotificationTokens { get; set; } = new List<NotificationToken>();

    public bool IsOnDuty { get; set; } = false;

    public bool CanBookAmenity()
        => Role.Type == UserRoleEnum.User && IsActive;
}