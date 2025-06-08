using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Amenity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TowerId { get; set; }
        public Tower Tower { get; set; }

        public ICollection<AmenityAvailability> Availabilities { get; set; } = new List<AmenityAvailability>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }

    public class AmenityAvailability
    {
        public int Id { get; set; }
        public int AmenityId { get; set; }
        public Amenity Amenity { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsWithinAvailability(TimeSpan time)
            => time >= StartTime && time < EndTime;
    }
}
