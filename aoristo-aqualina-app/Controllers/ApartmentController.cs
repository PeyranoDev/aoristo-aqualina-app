using Common.Models.Requests;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace aoristo_aqualina_app.Controllers
{
    [Route("apartments")]
    [ApiController]
    public class ApartmentController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateApartment([FromBody] ApartmentForCreateDTO dto)
        {
            return Ok();
        }
    }
}
