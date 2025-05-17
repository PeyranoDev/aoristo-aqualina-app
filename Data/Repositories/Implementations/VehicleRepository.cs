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
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AqualinaAPIContext _context;
        public VehicleRepository(AqualinaAPIContext context)
        {
            _context = context;
        }

        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            return await _context.Vehicles
                .Include(Vehicle => Vehicle.Requests)
                .FirstOrDefaultAsync(v => v.Id == id);
        }
        public async Task<bool> AddAsync(Vehicle vehicle)
        {
            try
            {
                await _context.Vehicles.AddAsync(vehicle);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Vehicle vehicle)
        {
            try
            {
                _context.Vehicles.Update(vehicle);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IQueryable<Vehicle> GetAll()
        {
            return _context.Vehicles.AsNoTracking();
        }

        public async Task<IList<Vehicle>> GetVehiclesPerUserIdAsync(int userId)
        {
            return await _context.Vehicles
                .Where(v => v.OwnerId == userId)
                .ToListAsync();
        }
    }
}
