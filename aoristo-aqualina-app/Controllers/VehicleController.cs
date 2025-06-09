using Common.Models.Requests;
using Common.Models.Responses;
using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Services.Main.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aoristo_aqualina_app.Controllers
{
    [Route("vehicle")]
    [ApiController]
    [EnableRateLimiting("ApiPolicy")] 
    public class VehicleController : MainController
    {
        private readonly IVehicleRequestService _vehicleRequestService;
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleRequestService vehicleRequestService, IVehicleService vehicleService)
        {
            _vehicleRequestService = vehicleRequestService;
            _vehicleService = vehicleService;
        }

        [HttpGet]
        [Authorize]
        [ResponseCache(Duration = 120, VaryByHeader = "Authorization")] 
        public async Task<IActionResult> GetSelfVehicles()
        {
            var userId = GetUserIdFromToken();
            var vehicles = await _vehicleService.GetVehiclesPerUserIdAsync(userId);

            if (vehicles == null || vehicles.Count == 0)
            {
                return NotFound(ApiResponse<object>.NotFound("No vehicles found for this user"));
            }

            return Ok(ApiResponse<IList<Vehicle>>.Ok(vehicles));
        }

        [HttpDelete("{vehicleId}")]
        [Authorize]
        public async Task<IActionResult> DeleteVehicle(int vehicleId)
        {
            var userId = GetUserIdFromToken();
            var role = GetUserRole();
            var vehicle = await _vehicleService.GetVehicleByIdAsync(vehicleId);

            if (vehicle == null)
            {
                return NotFound(ApiResponse<object>.NotFound("Vehicle not found"));
            }

            var isAdmin = role == "Admin";

            if (!isAdmin && vehicle.OwnerId != userId)
            {
                return Forbid();
            }

            if (await _vehicleService.HasActiveRequestAsync(vehicleId))
            {
                return BadRequest(ApiResponse<object>.Fail("Can't delete vehicles with active requests"));
            }

            var result = await _vehicleService.DeleteVehicleAsync(vehicleId);

            if (!result)
            {
                return BadRequest(ApiResponse<object>.Fail("Failed to delete the vehicle"));
            }

            return Ok(ApiResponse<string>.Ok("Vehicle deleted successfully"));
        }

        [HttpGet("security/active-requests")]
        [Authorize(Roles = "Admin,Security")]
        [ResponseCache(Duration = 30)] 
        public async Task<IActionResult> GetVehiclesForSecurity()
        {
            var vehicles = await _vehicleService.GetVehiclesWithActiveRequestsAsync();
            return Ok(ApiResponse<IList<Vehicle>>.Ok(vehicles));
        }

        [HttpGet("admin/get")]
        [Authorize(Roles = "Admin")]
        [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "*" })] 
        public async Task<IActionResult> GetVehiclesForAdmins([FromQuery] VehicleFilterParams filters, [FromQuery] PaginationParams pagination)
        {
            var pagedResult = await _vehicleService.GetVehiclesPagedAsync(filters, pagination);

            Response.Headers.Add("X-Total-Records", pagedResult.TotalRecords.ToString());
            Response.Headers.Add("X-Total-Pages", pagedResult.TotalPages.ToString());

            return Ok(ApiResponse<PagedResponse<VehicleForResponseDTO>>.Ok(pagedResult));
        }
    }
}
