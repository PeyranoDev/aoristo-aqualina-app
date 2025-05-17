using Common.Models.Requests;
using Common.Models.Responses.Common.Models.Responses;
using Common.Models.Responses;
using Data.Entities;
using Data.Repositories.Interfaces;
using Services.Main.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Common.Helpers;
using AutoMapper.QueryableExtensions;

namespace Services.Main.Implementations
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;

        public VehicleService(IVehicleRepository vehicleRepository, IMapper mapper)
        {
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
        }

        public async Task<IList<Vehicle>> GetVehiclesPerUserIdAsync(int userId)
        {
            return await _vehicleRepository.GetVehiclesPerUserIdAsync(userId);
        }

        public async Task<PagedResponse<VehicleForResponseDTO>> GetVehiclesPagedAsync(VehicleFilterParams filters,PaginationParams pagination)
        {
            var query = _vehicleRepository.GetAll();

            if (filters.IncludeRequests)
                query = query.Include(v => v.Requests);

            query = query
                .ApplyFilters(filters.Plate, filters.Model, filters.IsActive, filters.HasRequests)
                .ApplySorting(pagination.SortBy, pagination.SortOrder);

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ProjectTo<VehicleForResponseDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResponse<VehicleForResponseDTO>(
                data,
                totalRecords,
                pagination.PageNumber,
                pagination.PageSize
            );
        }
        public async Task<bool> DeleteVehicleAsync(int vehicleId)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

            if (vehicle == null)
            {
                return false;
            }
            vehicle.IsActive = false;
            if (await _vehicleRepository.UpdateAsync(vehicle)) { return true; }
            else { return false; }
        }
    }
}
