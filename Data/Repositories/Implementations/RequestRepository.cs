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
    public class RequestRepository : IRequestRepository
    {
        private readonly AqualinaAPIContext _context;

        public RequestRepository(AqualinaAPIContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Request request)
        {
            try
            {
                await _context.Requests.AddAsync(request);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Request request)
        {
            try
            {
                _context.Requests.Update(request);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Request>> GetAllAsync()
        {
            return await _context.Requests
                .Include(r => r.Vehicle)
                .Include(r => r.RequestedBy)
                .ToListAsync();
        }

        public async Task<Request?> GetLatestByVehicleAsync(int vehicleId)
        {
            return await _context.Requests
                .Include(r => r.Vehicle)
                .Include(r => r.RequestedBy)
                .Where(r => r.VehicleId == vehicleId)
                .OrderByDescending(r => r.RequestedAt)
                .FirstOrDefaultAsync();
        }
    }
}
