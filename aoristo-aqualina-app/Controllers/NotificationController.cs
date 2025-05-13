using Common.Models.Requests;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Main.Implementations;
using Services.Main.Interfaces;
using System.Text;

namespace aoristo_aqualina_app.Controllers
{
    [Route("notification")]
    [ApiController]
    public class NotificationsController : MainController
    {
        private readonly ITokenService _tokenService;

        public NotificationsController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        [Authorize]
        public async Task<IActionResult> RegisterPushToken([FromBody] NotificationTokenCreateDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Token))
            {
                return BadRequest("Token cannot be null or empty.");
            }

            var success = await _tokenService.AddNotificationTokenAsync(dto, GetUserIdFromToken());

            if (success)
            {
                return Ok("Token registered successfully.");
            }
            else
            {
                return StatusCode(500, "Failed to register token.");
            }

        }
    }
}
