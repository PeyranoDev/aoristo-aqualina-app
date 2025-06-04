using Common.Models.Requests;
using Common.Models.Responses;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Main.Interfaces;

namespace aoristo_aqualina_app.Controllers
{
    [Route("apartment")]
    [ApiController]
    public class ApartmentController : ControllerBase
    {
        private readonly IApartmentService _apartmentService;

        public ApartmentController(IApartmentService apartmentService)
        {
            _apartmentService = apartmentService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateApartment([FromBody] ApartmentForCreateDTO dto)
        {
            try
            {
                var apartment = await _apartmentService.CreateApartmentAsync(dto);
                return Ok(ApiResponse<Apartment>.Ok(apartment));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail($"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Security")]
        public async Task<IActionResult> GetAllApartments()
        {
            try
            {
                var apartments = await _apartmentService.GetAllApartmentsAsync();

                if (apartments == null || apartments.Count == 0)
                {
                    return NotFound(ApiResponse<object>.NotFound("No apartments found"));
                }

                return Ok(ApiResponse<List<Apartment>>.Ok(apartments));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail($"Internal server error: {ex.Message}"));
            }
        }
    }
}
