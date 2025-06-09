using AutoMapper;
using Common.Models;
using Common.Models.Requests;
using Common.Models.Responses;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Services.Main.Interfaces;
using System.Threading.Tasks;

namespace aoristo_aqualina_app.Controllers
{
    [Route("user")]
    [ApiController]
    [EnableRateLimiting("ApiPolicy")] 
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
            var updatedUser = await _userService.UpdateUserAsync(dto, GetUserIdFromToken());

            if (updatedUser == null)
                return BadRequest(ApiResponse<object>.Fail("User not found or you do not have permission to update this user."));

            return Ok(ApiResponse<object>.NoContent("User updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, User, Security")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<object>.NotFound($"User with id: {id}, not found"));

            if (user.Id != GetUserIdFromToken() && GetUserRole() != "Admin")
                return Forbid();

            await _userService.DeleteUserAsync(id);
            return Ok(ApiResponse<object>.NoContent("User deleted successfully"));
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, User, Security")]
        public async Task<IActionResult> DeleteCurrentUser()
        {
            await _userService.DeleteUserAsync(GetUserIdFromToken());
            return Ok(ApiResponse<object>.NoContent("User deleted successfully"));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User, Security")]
        [ResponseCache(Duration = 120, VaryByHeader = "Authorization")] 
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<object>.NotFound($"User with id: {id} not found"));

            if (user.Id != GetUserIdFromToken() && GetUserRole() != "Admin")
                return Forbid();

            var response = _mapper.Map<UserForResponse>(user);
            return Ok(ApiResponse<UserForResponse>.Ok(response));
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User, Security")]
        [ResponseCache(Duration = 120, VaryByHeader = "Authorization")] 
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userService.GetByIdAsync(GetUserIdFromToken());
            if (user == null)
                return NotFound(ApiResponse<object>.NotFound("User not found"));

            var response = _mapper.Map<UserForResponse>(user);
            return Ok(ApiResponse<UserForResponse>.Ok(response));
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "*" })] 
        public async Task<IActionResult> GetUsers(
            [FromQuery] PaginationParams pagination,
            [FromQuery] UserFilterParams filters)
        {
            var response = await _userService.GetUsersPagedAsync(filters, pagination);
            return Ok(ApiResponse<PagedResponse<UserForResponse>>.Ok(response, "Users fetched successfully"));
        }
    }
}
