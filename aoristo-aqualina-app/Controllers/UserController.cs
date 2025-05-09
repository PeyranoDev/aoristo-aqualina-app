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
    public class UserController : MainController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }


        [HttpPut]
        [Authorize(Roles = "Admin, User, Security")]
        public async Task<IActionResult> UpdateUser([FromBody] UserForUpdateDTO dto)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserAsync(dto, GetUserIdFromToken());
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
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                if (user.Id != GetUserIdFromToken() && GetUserRole() != "Admin")
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
            try
            {
                await _userService.DeleteUserAsync(GetUserIdFromToken());
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
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                if (user.Id != GetUserIdFromToken() && GetUserRole() != "Admin")
                {
                    return Forbid("You do not have permission to view this user.");
                }

                var response = _mapper.Map<UserForResponse>(user);
                return Ok(response);
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
            try
            {
                var user = await _userService.GetByIdAsync(GetUserIdFromToken());
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                var response = _mapper.Map<UserForResponse>(user);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResponse<UserForResponse>>> GetUsers([FromQuery] PaginationParams pagination,[FromQuery] UserFilterParams filters)
        {
            var response = await _userService.GetUsersPagedAsync(filters, pagination);
            return Ok(response);
        }
    }
}

