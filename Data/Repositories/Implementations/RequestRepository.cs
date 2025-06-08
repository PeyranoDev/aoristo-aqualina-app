using Data.Entities;
using Data.Enum;
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
                .ToListAsync();
        }

        public async Task<Request?> GetLatestByVehicleAsync(int vehicleId)
        {
            return await _context.Requests
                .Include(r => r.Vehicle)
                .Where(r => r.VehicleId == vehicleId)
                .OrderByDescending(r => r.RequestedAt)
                .FirstOrDefaultAsync();
        }
        public async Task<bool> DeleteOldRequestsAsync(TimeSpan olderThan)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.Subtract(olderThan);

                
                var oldRequests = await _context.Requests
                    .Where(r =>
                        (r.Status == VehicleRequestStatusEnum.Completed && r.CompletedAt < cutoffDate) ||
                        (r.Status == VehicleRequestStatusEnum.Cancelled && r.UpdatedAt < cutoffDate) ||
                        (r.Status == VehicleRequestStatusEnum.Pending && r.RequestedAt < cutoffDate.AddDays(-30)))
                    .ToListAsync();

                if (oldRequests.Count == 0) return false;

                _context.Requests.RemoveRange(oldRequests);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
    }
}
