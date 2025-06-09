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
    public class TowerRepository : ITowerRepository
    {
        private readonly AqualinaAPIContext _context;
        public TowerRepository(AqualinaAPIContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Simplemente devuelve el DbSet como IQueryable.
        /// AsNoTracking es una optimización para consultas de solo lectura.
        /// </summary>
        public IQueryable<Tower> GetAsQueryable()
        {
            return _context.Towers.AsNoTracking();
        }

        // --- Los otros métodos (GetByIdAsync, CreateAsync, etc.) permanecen igual ---

        public async Task<Tower?> GetByIdAsync(int id)
        {
            return await _context.Towers
                .Include(t => t.Apartments)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tower?> GetByNameAsync(string name)
        {
            return await _context.Towers
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }

        public async Task CreateAsync(Tower tower)
        {
            await _context.Towers.AddAsync(tower);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tower tower)
        {
            _context.Towers.Update(tower);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Tower tower)
        {
            _context.Towers.Remove(tower);
            await _context.SaveChangesAsync();
        }
    }
}
