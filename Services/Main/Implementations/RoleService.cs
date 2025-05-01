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
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<bool> RoleExistsAsync(int roleId)
        {
            return await _roleRepository.RoleExistsAsync(roleId);
        }

        public async Task<Role?> GetRoleByType(string type)
        {
            return await _roleRepository.GetRoleByType(type);
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }
    }
}
