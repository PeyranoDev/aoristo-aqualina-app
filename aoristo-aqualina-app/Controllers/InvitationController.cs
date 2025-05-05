using Common.Models.Requests;
using Common.Models.Responses;
using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Main.Interfaces;
using System.Security.Claims;

namespace aoristo_aqualina_app.Controllers
{
    [Route("invitation")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IInvitationService _invitationService;
        private readonly IRoleService _roleService;

        public InvitationController(IUserService userService,  
            IInvitationService invitationService,
            IRoleService roleService
            )
        {
            _userService = userService;
            _invitationService = invitationService;
            _roleService = roleService;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        private string GetUserRole()
        {
            return User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> CreateInvitation([FromBody] CreateInvitationDto dto)
        {
            var userId = GetUserIdFromToken();

            var roleExists = await _roleService.RoleExistsAsync(dto.RoleId);
            if (!roleExists) return BadRequest("Invalid role");


            var emailExists = await _userService.EmailExistsAsync(dto.Email);
            if (emailExists) return BadRequest("Email already registered");

            var token = _invitationService.CreateInvitationAsync(dto, userId);

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var inviteUrl = $"{baseUrl}/register?token={token}";

            return Ok(new { InvitationUrl = inviteUrl });
        }

        [HttpGet("validate")]
        public async Task<IActionResult> ValidateInvitationToken([FromQuery] string token)
        {
            var invitation = await _invitationService.GetInvitationAsync(token);

            if (invitation == null) return NotFound("Invalid invitation token");
            if (invitation.IsUsed) return BadRequest("Invitation already used");
            if (invitation.ExpiresAt < DateTime.UtcNow) return BadRequest("Invitation has expired");

            return Ok(new
            {
                Email = invitation.Email,
                Role = invitation.Role.Type.ToString(),
                Valid = true
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterWithToken([FromBody] RegisterWithTokenDto dto)
        {
            var invitation = await _invitationService.GetInvitationAsync(dto.Token);

            if (invitation == null) return NotFound("Invalid invitation token");
            if (invitation.IsUsed) return BadRequest("Invitation already used");
            if (invitation.ExpiresAt < DateTime.UtcNow) return BadRequest("Invitation has expired");
            if (invitation.Email != dto.Email) return BadRequest("Email doesn't match invitation");

            if (await _userService.EmailExistsAsync(dto.Email) && await _userService.UsernameExistsAsync(dto.Username))
                return BadRequest("Email already registered");


            var user = await _userService.CreateUserAsyncWithInvitation(dto, invitation.RoleId, invitation.ApartmentId);

            invitation.IsUsed = true;

            var id = await _invitationService.UpdateInvitationAsync(invitation);   


            return Ok(id);
        }
    }
}
