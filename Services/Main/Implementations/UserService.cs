using Common.Models;
using Data.Entities;
using Data.Repositories.Interfaces;
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
        private readonly IHashingService _hashingService;

        public UserService(IUserRepository userRepo, IHashingService hashingService)
        {
            _userRepo = userRepo;
            _hashingService = hashingService;
        }
        public User? Validate(CreedentialsDTO dto)
        {
            User? user = _userRepo.Validate(dto.Username, _hashingService.HashPassword(dto.Password));
            return user;
        }

        public User? UpdateUser(UserForUpdateDTO dto, int userid) 
        { 
            _userRepo.Update(userid, dto);
        }
    }
}
