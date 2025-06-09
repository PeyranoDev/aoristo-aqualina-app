using Common.Models.Requests;
using Common.Models.Responses;

namespace Services.Main.Interfaces
{
    public interface ITowerService
    {
        Task<TowerForUserResponseDTO> CreateTowerAsync(TowerForCreateDTO towerDto);
        Task DeleteTowerAsync(int id);
        Task<PagedResponse<TowerForUserResponseDTO>> GetPublicTowerListAsync(TowerFilterParams filterParams);
        Task<TowerForUserResponseDTO> GetTowerByIdAsync(int id);
        Task UpdateTowerAsync(int id, TowerForUpdateDTO towerDto);
    }
}