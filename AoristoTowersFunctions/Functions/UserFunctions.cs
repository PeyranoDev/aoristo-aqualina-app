using AoristoTowersFunctions.Helpers;
using AoristoTowersFunctions.Middleware;
using AutoMapper;
using Common.Models.Requests;
using Common.Models.Responses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Services.Main.Interfaces;
using System.Net;
using System.Web;

namespace AoristoTowersFunctions.Functions
{
    public class UserFunctions
    {
        private readonly ILogger<UserFunctions> _logger;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserFunctions(ILogger<UserFunctions> logger, IUserService userService, IMapper mapper)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
        }

        [Function("GetCurrentUser")]
        [Authorize(Roles = "Admin,Security,User")]
        public async Task<HttpResponseData> GetCurrentUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user")] HttpRequestData req,
            FunctionContext context)
        {
            var userId = context.GetUserId();
            _logger.LogInformation("Fetching current user with ID: {UserId}", userId);

            var user = await _userService.GetByIdAsync(userId);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<UserForResponse>.Ok(user));
        }

        [Function("GetUserById")]
        [Authorize(Roles = "Admin,Security,User")]
        public async Task<HttpResponseData> GetUserById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{id:int}")] HttpRequestData req,
            int id, FunctionContext context)
        {
            _logger.LogInformation("Fetching user by ID: {TargetId}", id);
            var requestingUserId = context.GetUserId();
            var requestingUserRole = context.GetUserRole();

            if (requestingUserId != id && requestingUserRole != "Admin")
            {
                return await req.CreateJsonResponse(HttpStatusCode.Forbidden, ApiResponse<object>.Fail("Access denied."));
            }

            var user = await _userService.GetByIdAsync(id);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<UserForResponse>.Ok(user));
        }

        [Function("UpdateUser")]
        [Authorize(Roles = "Admin,Security,User")]
        public async Task<HttpResponseData> UpdateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user")] HttpRequestData req,
            FunctionContext context)
        {
            var userId = context.GetUserId();
            _logger.LogInformation("Updating user with ID: {UserId}", userId);

            var updateDto = await req.ReadFromJsonAsync<UserForUpdateDTO>();
            if (updateDto == null)
            {
                return await req.CreateJsonResponse(HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Invalid request body."));
            }

            await _userService.UpdateUserAsync(updateDto, userId);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<object>.NoContent("User updated successfully."));
        }

        [Function("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseData> GetAllUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/all")] HttpRequestData req)
        {
            _logger.LogInformation("Fetching all users (Admin).");

            var queryParams = HttpUtility.ParseQueryString(req.Url.Query);
            var pagination = new PaginationParams
            {
                PageNumber = int.TryParse(queryParams["pageNumber"], out var pageNum) ? pageNum : 1,
                PageSize = int.TryParse(queryParams["pageSize"], out var pageSize) ? pageSize : 10,
                SortBy = queryParams["sortBy"] ?? "Name",
                SortOrder = queryParams["sortOrder"] ?? "asc"
            };

            var filters = new UserFilterParams
            {
                Name = queryParams["name"],
                Email = queryParams["email"],
                RoleType = queryParams["roleType"],
                IsActive = bool.TryParse(queryParams["isActive"], out var isActive) ? isActive : null,
                ApartmentIdentifier = queryParams["apartmentIdentifier"],
                TowerId = int.TryParse(queryParams["towerId"], out var towerId) ? towerId : null
            };

            var pagedResult = await _userService.GetUsersPagedAsync(filters, pagination);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<PagedResponse<UserForResponse>>.Ok(pagedResult));
        }

        [Function("DeleteCurrentUser")]
        [Authorize]
        public async Task<HttpResponseData> DeleteCurrentUser(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "user")] HttpRequestData req,
           FunctionContext context)
        {
            var userId = context.GetUserId();
            _logger.LogInformation("Request to delete current user with ID: {UserId}", userId);
            await _userService.DeleteUserAsync(userId);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<object>.NoContent("Current user deleted successfully."));
        }


        [Function("DeleteUserById")]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseData> DeleteUserById(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "user/{id:int}")] HttpRequestData req,
           int id, FunctionContext context)
        {
            _logger.LogInformation("Request to delete user with ID: {TargetId} by Admin {AdminId}", id, context.GetUserId());
            await _userService.DeleteUserAsync(id);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<object>.NoContent($"User with ID {id} deleted successfully."));
        }
    }
}
