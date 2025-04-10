using Common.Models;
using Services.Main.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Main.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
        public User? Validate(CreedentialsDTO dto)
        {
            User? user = _userRepo.Validate(dto.Email, dto.Password);
            return user;
        }
    }
}
