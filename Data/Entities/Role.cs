using Data.Enum;

namespace Data.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public UserRoleEnum Type { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
