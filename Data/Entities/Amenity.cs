namespace Data.Entities
{
    public class Amenity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<AmenityAvailability> Availabilities { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }

    public class AmenityAvailability
    {
        public int Id { get; set; }

        public int AmenityId { get; set; }
        public Amenity Amenity { get; set; }

        public DayOfWeek DayOfWeek { get; set; } 

        public TimeSpan StartTime { get; set; }  
        public TimeSpan EndTime { get; set; }   
    }
}
