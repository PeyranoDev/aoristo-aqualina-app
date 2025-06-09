using Common.Models.Requests;
using Common.Models.Responses;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Services.Main.Interfaces;

namespace aoristo_aqualina_app.Controllers
{
    [Route("towers")]
    [ApiController]
    [EnableRateLimiting("ApiPolicy")]
    public class TowerController : MainController
    {
        private readonly ITowerService _towerService;

        public TowerController(ITowerService towerService)
        {
            _towerService = towerService;
        }


        [HttpGet("public-list")]
        [AllowAnonymous]
        [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "*" })]
        public async Task<IActionResult> GetPublicTowerList([FromQuery] TowerFilterParams filterParams)
        {
            var pagedTowers = await _towerService.GetPublicTowerListAsync(filterParams);
            var response = ApiResponse<PagedResponse<TowerForUserResponseDTO>>.Ok(pagedTowers, "Lista de torres obtenida correctamente.");
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ResponseCache(Duration = 600)] 
        public async Task<IActionResult> GetTowerById(int id)
        {
            var tower = await _towerService.GetTowerByIdAsync(id);
            var response = ApiResponse<TowerForUserResponseDTO>.Ok(tower, "Torre encontrada.");
            return Ok(response);
        }


        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateTower([FromBody] TowerForCreateDTO towerDto)
        {
            var createdTower = await _towerService.CreateTowerAsync(towerDto);
            var response = ApiResponse<TowerForUserResponseDTO>.Ok(createdTower, "Torre creada exitosamente.");

            return CreatedAtAction(nameof(GetTowerById), new { id = createdTower.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateTower(int id, [FromBody] TowerForUpdateDTO towerDto)
        {
            await _towerService.UpdateTowerAsync(id, towerDto);
            var response = ApiResponse<object>.NoContent("Torre actualizada correctamente.");
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTower(int id)
        {
            await _towerService.DeleteTowerAsync(id);
            var response = ApiResponse<object>.NoContent("Torre eliminada correctamente.");
            return Ok(response);
        }
    }
}
