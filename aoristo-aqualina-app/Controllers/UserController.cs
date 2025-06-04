using AutoMapper;
using Common.Models;
using Common.Models.Requests;
using Common.Models.Responses;
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
                    return BadRequest(ApiResponse<object>.Fail("User not found or you do not have permission to update this user."));
                }
                return Ok(ApiResponse<object>.NoContent("User updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FromException(ex, "There was an internal server error"));
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
                    return NotFound(ApiResponse<object>.NotFound($"User with id: {id}, not found"));
                }

                if (user.Id != GetUserIdFromToken() && GetUserRole() != "Admin")
                {
                    return Forbid();
                }

                await _userService.DeleteUserAsync(id);
                return Ok(ApiResponse<object>.NoContent("User deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FromException(ex, "There was an internal server error"));
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, User, Security")]
        public async Task<IActionResult> DeleteCurrentUser()
        {
            try
            {
                await _userService.DeleteUserAsync(GetUserIdFromToken());
                return Ok(ApiResponse<object>.NoContent("User deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FromException(ex, "There was an internal server error"));
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
                    return NotFound(ApiResponse<object>.NotFound($"User with id: {id} not found"));
                }

                if (user.Id != GetUserIdFromToken() && GetUserRole() != "Admin")
                {
                    return Forbid();
                }

                var response = _mapper.Map<UserForResponse>(user);
                return Ok(ApiResponse<UserForResponse>.Ok(response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FromException(ex, "Internal server error"));
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
                    return NotFound(ApiResponse<object>.NotFound("User not found"));
                }
                var response = _mapper.Map<UserForResponse>(user);
                return Ok(ApiResponse<UserForResponse>.Ok(response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FromException(ex, "Internal server error"));
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResponse<UserForResponse>>> GetUsers([FromQuery] PaginationParams pagination, [FromQuery] UserFilterParams filters)
        {
            try
            {
                var response = await _userService.GetUsersPagedAsync(filters, pagination);
                return Ok(ApiResponse<PagedResponse<UserForResponse>>.Ok(response, "Users fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FromException(ex, "Internal server error"));
            }
        }
    }
}
