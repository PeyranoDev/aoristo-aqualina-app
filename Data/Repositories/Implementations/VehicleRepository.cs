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

        public async Task<bool> HasActiveRequestAsync(int vehicleId)
        {
            var activeStatuses = new[]
            {
                VehicleRequestStatusEnum.Pending,
                VehicleRequestStatusEnum.InPreparation,
                VehicleRequestStatusEnum.AlmostReady,
                VehicleRequestStatusEnum.Ready
            };

            return await _context.Requests
                .AnyAsync(r => r.VehicleId == vehicleId && activeStatuses.Contains(r.Status));
        }

        public async Task<Request?> GetLastActiveRequestAsync(int vehicleId)
        {
            var activeStatuses = new[]
            {
            VehicleRequestStatusEnum.Pending,
            VehicleRequestStatusEnum.InPreparation,
            VehicleRequestStatusEnum.AlmostReady,
            VehicleRequestStatusEnum.Ready
            };

            return await _context.Requests
                .Where(r => r.VehicleId == vehicleId && activeStatuses.Contains(r.Status))
                .OrderByDescending(r => r.RequestedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IList<Vehicle>> GetVehiclesPerUserIdWithLastRequestAsync(int userId)
        {
            return await _context.Vehicles
                .Where(v => v.OwnerId == userId)
                .Select(v => new Vehicle
                {
                    Id = v.Id,
                    Plate = v.Plate,
                    Model = v.Model,
                    Color = v.Color,
                    IsParked = v.IsParked,
                    IsActive = v.IsActive,
                    OwnerId = v.OwnerId,
                    Requests = v.Requests
                        .OrderByDescending(r => r.RequestedAt)
                        .Take(1)
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<IList<Vehicle>> GetVehiclesWithActiveRequestsAsync()
        {
            var activeStatuses = new[]
            {
                VehicleRequestStatusEnum.Pending,
                VehicleRequestStatusEnum.InPreparation,
                VehicleRequestStatusEnum.AlmostReady,
                VehicleRequestStatusEnum.Ready
            };

            return await _context.Vehicles
                .Where(v => v.Requests.Any(r => activeStatuses.Contains(r.Status)))
                .Include(v => v.Requests
                .Where(r => activeStatuses.Contains(r.Status)))
                .ToListAsync();
        }
    }
}
