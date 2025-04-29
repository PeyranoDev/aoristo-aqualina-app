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
    public required bool IsActive { get; set; }
    public required string Phone { get; set; } 
    public Apartment? Apartment { get; set; }
    public ICollection<Reservation>? Reservations { get; set; }
    public ICollection<Vehicle>? OwnedCars { get; set; }
}