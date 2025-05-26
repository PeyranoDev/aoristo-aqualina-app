using Common.Models.Requests;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Main.Interfaces;

namespace aoristo_aqualina_app.Controllers
{
    [Route("vehicle")]
    [ApiController]
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
        public async Task<IActionResult> GetSelfVehicles()
        {
            var userId = GetUserIdFromToken();

            var vehicles = await _vehicleService.GetVehiclesPerUserIdAsync(userId);

            if (vehicles == null)
            {
                return BadRequest("Invalid role");
            }
            return Ok(vehicles);
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
                return NotFound("Vehicle not found");
            }

            var isAdmin = role == "Admin";

            if (!isAdmin && vehicle.OwnerId != userId)
            {
                return BadRequest("You are not authorized to delete this vehicle");
            }

            if (await _vehicleService.HasActiveRequestAsync(vehicleId))
            {
                return BadRequest("Cannot delete vehicle with active requests");
            }

            var result = await _vehicleService.DeleteVehicleAsync(vehicleId);

            if (!result)
            {
                return BadRequest("Failed to delete vehicle");
            }

            return Ok("Vehicle deleted successfully");
        }

        [HttpGet("security/active-requests")]
        [Authorize(Roles = "Admin,Security")]

        public async Task<IActionResult> GetVehiclesForSecurity()
        {
            var vehicles = await _vehicleService.GetVehiclesWithActiveRequestsAsync();
            return Ok(vehicles);
        }

        [HttpGet("/admin/get")]
        [Authorize(Roles="Admin")]

        public async Task<IActionResult> GetVehiclesForAdmins([FromQuery] VehicleFilterParams filters,
        [FromQuery] PaginationParams pagination)
        {
            var pagedResult = await _vehicleService.GetVehiclesPagedAsync(filters, pagination);

            Response.Headers.Add("X-Total-Records", pagedResult.TotalRecords.ToString());
            Response.Headers.Add("X-Total-Pages", pagedResult.TotalPages.ToString());

            return Ok(pagedResult);
        }
    }
}
