using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;
using Data.Repositories.Interfaces;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AqualinaAPIContext _context;
        public UserRepository(AqualinaAPIContext context)
        {
            _context = context;
        }

        public User? Validate(string username, string password)
        {
            {
                return _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Username == username && u.PasswordHash == password);
            }
        }
    }
}
