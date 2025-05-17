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

        [HttpGet]
        [Route("/security/get")]
        [Authorize(Roles ="Admin, Security")]

        public async Task<IActionResult> GetVehiclesForSecurity()
        {
            var vehicles = await _vehicleService.GetVehiclesWithoutRequestsAsync();
            return Ok(vehicles);

        }

        [HttpGet]
        [Route("/admin/get")]
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
