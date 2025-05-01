using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AqualinaAPIContext _context;

        public RoleRepository(AqualinaAPIContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> RoleExistsAsync(int roleId)
        {
            return await _context.Roles.AnyAsync(r => r.Id == roleId);
        }

        public async Task<Role?> GetRoleByType(string type)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Type.ToString() == type);
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }
    }
}
