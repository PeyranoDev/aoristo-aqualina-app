using AoristoTowersFunctions.Helpers;
using AoristoTowersFunctions.Middleware;
using Common.Models.Requests;
using Common.Models.Responses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Services.Main.Interfaces;
using System.Net;
using System.Web;

namespace AoristoTowersFunctions.Functions
{
    public class TowerFunctions
    {
        private readonly ILogger<TowerFunctions> _logger;
        private readonly ITowerService _towerService;

        public TowerFunctions(ILogger<TowerFunctions> logger, ITowerService towerService)
        {
            _logger = logger;
            _towerService = towerService;
        }

        [Function("GetPublicTowerList")]
        public async Task<HttpResponseData> GetPublicTowerList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "towers/public-list")] HttpRequestData req)
        {
            _logger.LogInformation("Fetching public list of towers.");

            var queryParams = HttpUtility.ParseQueryString(req.Url.Query);
            var filterParams = new TowerFilterParams
            {
                Name = queryParams["name"],
                PageNumber = int.TryParse(queryParams["pageNumber"], out var pageNum) ? pageNum : 1,
                PageSize = int.TryParse(queryParams["pageSize"], out var pageSize) ? pageSize : 10
            };

            var pagedTowers = await _towerService.GetPublicTowerListAsync(filterParams);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<PagedResponse<TowerForUserResponseDTO>>.Ok(pagedTowers));
        }

        [Function("GetTowerById")]
        public async Task<HttpResponseData> GetTowerById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "towers/{id:int}")] HttpRequestData req, int id)
        {
            _logger.LogInformation("Fetching tower by ID: {TowerId}", id);
            var tower = await _towerService.GetTowerByIdAsync(id);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<TowerForUserResponseDTO>.Ok(tower));
        }

        [Function("CreateTower")]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseData> CreateTower(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "towers")] HttpRequestData req)
        {
            _logger.LogInformation("Create tower request received.");

            var towerDto = await req.ReadFromJsonAsync<TowerForCreateDTO>();
            if (towerDto == null)
            {
                return await req.CreateJsonResponse(HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Invalid request body."));
            }

            var createdTower = await _towerService.CreateTowerAsync(towerDto);
            var response = await req.CreateJsonResponse(HttpStatusCode.Created, ApiResponse<TowerForUserResponseDTO>.Ok(createdTower, "Tower created successfully."));
            response.Headers.Add("Location", $"/api/towers/{createdTower.Id}");
            return response;
        }

        [Function("UpdateTower")]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseData> UpdateTower(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "towers/{id:int}")] HttpRequestData req, int id)
        {
            _logger.LogInformation("Update tower request for ID: {TowerId}", id);

            var towerDto = await req.ReadFromJsonAsync<TowerForUpdateDTO>();
            if (towerDto == null)
            {
                return await req.CreateJsonResponse(HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Invalid request body."));
            }

            await _towerService.UpdateTowerAsync(id, towerDto);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<object>.NoContent("Tower updated successfully."));
        }

        [Function("DeleteTower")]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseData> DeleteTower(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "towers/{id:int}")] HttpRequestData req, int id)
        {
            _logger.LogInformation("Delete tower request for ID: {TowerId}", id);
            await _towerService.DeleteTowerAsync(id);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<object>.NoContent("Tower deleted successfully."));
        }
    }
}
