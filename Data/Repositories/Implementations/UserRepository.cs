using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Implementations
{
    public class UserRepository
    {
        private readonly AqualinaAPIContext _context;
        public UserRepository(AqualinaAPIContext context)
        {
            _context = context;
        }

        public User? Validate(string user, string password)
        {
            {
                return _context.Users.FirstOrDefault(x => x.Email == user && x.PasswordHash == password);
            }
        }
    }
}
