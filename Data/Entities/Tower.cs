using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Tower
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Adress { get; set; } = string.Empty;
        public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
        public ICollection<News> News { get; set; } = new List<News>();
        public AppSettings Settings { get; set; }
        public ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
        public ICollection<UserTower> UserTowers { get; set; } = new List<UserTower>();
    }
}