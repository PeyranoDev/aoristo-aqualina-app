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
            var apartment = await _apartmentService.CreateApartmentAsync(dto);
            return Ok(ApiResponse<Apartment>.Ok(apartment));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Security")]
        public async Task<IActionResult> GetAllApartments()
        {
            var apartments = await _apartmentService.GetAllApartmentsAsync();

            if (apartments == null || apartments.Count == 0)
                return NotFound(ApiResponse<object>.NotFound("No apartments found"));

            return Ok(ApiResponse<List<Apartment>>.Ok(apartments));
        }

        [HttpDelete("{apartmentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteApartmentById([FromRoute] int apartmentId)
        {
            byte result = await _apartmentService.DeleteApartmentAsync(apartmentId);

            return result switch
            {
                0 => NotFound(ApiResponse<object>.NotFound($"Apartment with id: {apartmentId} not found")),
                2 => BadRequest(ApiResponse<object>.ValidationError($"Apartment with id: {apartmentId} cannot be deleted because it is already deactivated")),
                _ => Ok(ApiResponse<object>.NoContent("Apartment deleted successfully")),
            };
        }

        [HttpPut("{apartmentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateApartment([FromRoute] int apartmentId, [FromBody] Apartment apartment)
        {
            if (apartment.Id != apartmentId)
                return BadRequest(ApiResponse<object>.ValidationError("Apartment ID mismatch"));

            var updatedApartment = await _apartmentService.UpdateApartmentAsync(apartment);

            if (updatedApartment == null)
                return NotFound(ApiResponse<object>.NotFound($"Apartment with id: {apartmentId} not found"));

            return Ok(ApiResponse<Apartment>.Ok(updatedApartment));
        }

        [HttpGet("{apartmentId}")]
        [Authorize(Roles = "Admin,Security")]
        public async Task<IActionResult> GetApartmentById([FromRoute] int apartmentId)
        {
            var apartment = await _apartmentService.GetApartmentByIdAsync(apartmentId);

            if (apartment == null)
                return NotFound(ApiResponse<object>.NotFound($"Apartment with id: {apartmentId} not found"));

            return Ok(ApiResponse<Apartment>.Ok(apartment));
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin,Security")]
        public async Task<IActionResult> GetApartmentByUserId([FromRoute] int userId)
        {
            var apartment = await _apartmentService.GetApartmentsByUserIdAsync(userId);
            return Ok(ApiResponse<Apartment>.Ok(apartment));
        }
    }
}
