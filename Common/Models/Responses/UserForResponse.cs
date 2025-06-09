using Common.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Responses
{
    public class UserForResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public ApartmentInfoDTO? ApartmentInfo { get; set; }
        public ICollection<TowerForUserResponseDTO> AssociatedTowers { get; set; } = new List<TowerForUserResponseDTO>();
    }
}
