using AutoMapper;
using Common.Models;
using Common.Models.Requests;
using Common.Models.Responses.Common.Models.Responses;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.Main.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace aoristo_aqualina_app.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IUserContextService userContextService, IMapper mapper)
        {
            _userService = userService;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        [HttpPut]
        [Authorize(Roles = "Admin, User, Security")]
        public async Task<IActionResult> UpdateUser([FromBody] UserForUpdateDTO dto)
        {
            var userId = _userContextService.GetUserId();
            try
            {
                var updatedUser = await _userService.UpdateUserAsync(dto, userId);
                if (updatedUser == null)
                {
                    return NotFound("User not found.");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, User, Security")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var userId = _userContextService.GetUserId();
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                if (user.Id != userId && _userContextService.GetUserRole() != "Admin")
                {
                    return Forbid("You do not have permission to delete this user.");
                }

                await _userService.DeleteUserAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Admin, User, Security")]
        public async Task<IActionResult> DeleteCurrentUser()
        {
            var userId = _userContextService.GetUserId();
            try
            {
                await _userService.DeleteUserAsync(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User, Security")]
        public async Task<IActionResult> GetUser(int id)
        {
            var userId = _userContextService.GetUserId();
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                if (user.Id != userId && _userContextService.GetUserRole() != "Admin")
                {
                    return Forbid("You do not have permission to view this user.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin, User, Security")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = _userContextService.GetUserId();
            try
            {
                var user = await _userService.GetByIdAsync(userId);
                var response = _mapper.Map<UserForResponse>(user);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResponse<UserForResponse>>> GetUsers([FromQuery] PaginationParams pagination,[FromQuery] UserFilterParams filters)
        {
            var response = await _userService.GetUsersPagedAsync(filters, pagination);
            return Ok(response);
        }
    }
}

