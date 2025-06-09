using AutoMapper;
using Common.Exceptions;
using Common.Helpers;
using Common.Models.Requests;
using Common.Models.Responses;
using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Services.Main.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Main.Implementations
{
    public class TowerService : ITowerService
    {
        private readonly ITowerRepository _towerRepo;
        private readonly IMapper _mapper;

        public TowerService(ITowerRepository towerRepo, IMapper mapper)
        {
            _towerRepo = towerRepo;
            _mapper = mapper;
        }

        public async Task<PagedResponse<TowerForUserResponseDTO>> GetPublicTowerListAsync(TowerFilterParams filterParams)
        {
            var query = _towerRepo.GetAsQueryable();
            query = query.ApplyFilters(filterParams).ApplySorting(filterParams.SortBy, filterParams.SortOrder);

            var totalCount = await query.CountAsync();
            var towerEntities = await query
                .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                .Take(filterParams.PageSize)
                .ToListAsync();

            var towerDtos = _mapper.Map<List<TowerForUserResponseDTO>>(towerEntities);
            return new PagedResponse<TowerForUserResponseDTO>(towerDtos, totalCount, filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<TowerForUserResponseDTO> GetTowerByIdAsync(int id)
        {
            var tower = await _towerRepo.GetByIdAsync(id);
            if (tower == null) throw new NotFoundException("Tower", id);
            return _mapper.Map<TowerForUserResponseDTO>(tower);
        }


        public async Task<TowerForUserResponseDTO> CreateTowerAsync(TowerForCreateDTO towerDto)
        {
            var existingTower = await _towerRepo.GetByNameAsync(towerDto.Name);
            if (existingTower != null) throw new TowerAlreadyExistsException(towerDto.Name);

            var newTower = _mapper.Map<Tower>(towerDto);
            await _towerRepo.CreateAsync(newTower);

            return _mapper.Map<TowerForUserResponseDTO>(newTower);
        }

        public async Task UpdateTowerAsync(int id, TowerForUpdateDTO towerDto)
        {
            var existingTower = await _towerRepo.GetByIdAsync(id);
            if (existingTower == null) throw new NotFoundException("Tower", id);

            if (towerDto.Name != null && towerDto.Name.ToLower() != existingTower.Name.ToLower())
            {
                var towerWithSameName = await _towerRepo.GetByNameAsync(towerDto.Name);
                if (towerWithSameName != null && towerWithSameName.Id != id)
                {
                    throw new TowerAlreadyExistsException(towerDto.Name);
                }
            }

            _mapper.Map(towerDto, existingTower);
            await _towerRepo.UpdateAsync(existingTower);
        }

        public async Task DeleteTowerAsync(int id)
        {
            var towerToDelete = await _towerRepo.GetByIdAsync(id);
            if (towerToDelete == null) throw new NotFoundException("Tower", id);

            if (towerToDelete.Apartments.Any()) throw new TowerInUseException(id);

            await _towerRepo.DeleteAsync(towerToDelete);
        }
    }
}
