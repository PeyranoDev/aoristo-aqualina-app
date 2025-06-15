using AoristoTowersFunctions.Helpers;
using AoristoTowersFunctions.Middleware;
using Common.Models.Requests;
using Common.Models.Responses;
using Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Services.Main.Interfaces;
using System.Net;
using System.Web;

namespace AoristoTowersFunctions.Functions
{
    public class VehicleFunctions
    {
        private readonly ILogger<VehicleFunctions> _logger;
        private readonly IVehicleService _vehicleService;

        public VehicleFunctions(ILogger<VehicleFunctions> logger, IVehicleService vehicleService)
        {
            _logger = logger;
            _vehicleService = vehicleService;
        }

        [Function("GetSelfVehicles")]
        [Authorize]
        public async Task<HttpResponseData> GetSelfVehicles(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "vehicle")] HttpRequestData req,
            FunctionContext context)
        {
            var userId = context.GetUserId();
            _logger.LogInformation("Fetching vehicles for user ID: {UserId}", userId);

            var vehicles = await _vehicleService.GetVehiclesPerUserIdAsync(userId);

            if (vehicles == null || vehicles.Count == 0)
            {
                return await req.CreateJsonResponse(HttpStatusCode.NotFound, ApiResponse<object>.NotFound("No vehicles found for this user."));
            }

            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<IList<Vehicle>>.Ok(vehicles));
        }

        [Function("DeleteVehicle")]
        [Authorize]
        public async Task<HttpResponseData> DeleteVehicle(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "vehicle/{vehicleId:int}")] HttpRequestData req,
            int vehicleId, FunctionContext context)
        {
            var userId = context.GetUserId();
            var userRole = context.GetUserRole();
            _logger.LogInformation("User {UserId} attempting to delete vehicle {VehicleId}", userId, vehicleId);

            var vehicle = await _vehicleService.GetVehicleByIdAsync(vehicleId);
            if (vehicle == null)
            {
                return await req.CreateJsonResponse(HttpStatusCode.NotFound, ApiResponse<object>.NotFound("Vehicle not found."));
            }

            if (userRole != "Admin" && vehicle.OwnerId != userId)
            {
                return await req.CreateJsonResponse(HttpStatusCode.Forbidden, ApiResponse<object>.Fail("Access denied."));
            }

            if (await _vehicleService.HasActiveRequestAsync(vehicleId))
            {
                return await req.CreateJsonResponse(HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Cannot delete vehicles with active requests."));
            }

            await _vehicleService.DeleteVehicleAsync(vehicleId);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<object>.Ok("Vehicle deleted successfully."));
        }

        [Function("GetVehiclesForSecurity")]
        [Authorize(Roles = "Admin,Security")]
        public async Task<HttpResponseData> GetVehiclesForSecurity(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "vehicle/security/active-requests")] HttpRequestData req)
        {
            _logger.LogInformation("Fetching vehicles with active requests for security.");
            var vehicles = await _vehicleService.GetVehiclesWithActiveRequestsAsync();
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<IList<Vehicle>>.Ok(vehicles));
        }

        [Function("GetVehiclesForAdmins")]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseData> GetVehiclesForAdmins(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "vehicle/admin/get")] HttpRequestData req)
        {
            _logger.LogInformation("Fetching paged vehicles for admin.");

            var queryParams = HttpUtility.ParseQueryString(req.Url.Query);
            var pagination = new PaginationParams
            {
                PageNumber = int.TryParse(queryParams["pageNumber"], out var pageNum) ? pageNum : 1,
                PageSize = int.TryParse(queryParams["pageSize"], out var pageSize) ? pageSize : 10,
                SortBy = queryParams["sortBy"] ?? "Model",
                SortOrder = queryParams["sortOrder"] ?? "asc"
            };

            var filters = new VehicleFilterParams
            {
                Plate = queryParams["plate"],
                Model = queryParams["model"],
                IsActive = bool.TryParse(queryParams["isActive"], out var isActive) ? isActive : null,
                HasRequests = bool.TryParse(queryParams["hasRequests"], out var hasRequests) ? hasRequests : null,
                IncludeRequests = bool.TryParse(queryParams["includeRequests"], out var includeRequests) && includeRequests
            };

            var pagedResult = await _vehicleService.GetVehiclesPagedAsync(filters, pagination);

            var response = await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<PagedResponse<VehicleForResponseDTO>>.Ok(pagedResult));
            response.Headers.Add("X-Total-Records", pagedResult.TotalRecords.ToString());
            response.Headers.Add("X-Total-Pages", pagedResult.TotalPages.ToString());

            return response;
        }
    }
}
