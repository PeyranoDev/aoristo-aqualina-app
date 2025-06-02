using Common.Models.Requests;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Main.Implementations;

namespace aoristo_aqualina_app.Controllers
{
    [Route("apartment")]
    [ApiController]
    public class ApartmentController : ControllerBase
    {
        private readonly ApartmentService _apartmentService;
        public ApartmentController(ApartmentService apartmentService)
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
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Security")]
        public async Task<IActionResult> GetAllApartments()
        {
            try
            {
                var apartments = await _apartmentService.GetAllApartmentsAsync();
                return Ok(apartments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
