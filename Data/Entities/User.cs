using Data.Entities;

public class User
{
    public int Id { get; set; }
    public required string Email { get; set; } 
    public required string Username { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string PasswordHash { get; set; }
    public Role Role { get; set; }
    public int RoleId { get; set; }
    public required bool IsActive { get; set; }
    public required string Phone { get; set; } 
    public Apartment? Apartment { get; set; }
    public int? ApartmentId { get; set; }
    public ICollection<Request>? Requests { get; set; }
    public ICollection<Reservation>? Reservations { get; set; }
    public ICollection<Vehicle>? OwnedCars { get; set; }
    public ICollection<NotificationToken>? NotificationTokens { get; set; }
    public bool? IsOnDuty { get; set; } = false;
}