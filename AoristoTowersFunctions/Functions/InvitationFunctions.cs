using AoristoTowersFunctions.Helpers;
using AoristoTowersFunctions.Middleware;
using Common.Models.Requests;
using Common.Models.Responses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Services.Main.Interfaces;
using System.Net;
using System.Threading.Tasks;

namespace AoristoTowersFunctions.Functions
{
    public class InvitationFunctions
    {
        private readonly IInvitationService _invitationService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ILogger<InvitationFunctions> _logger;

        public InvitationFunctions(IInvitationService invitationService, IUserService userService, IRoleService roleService, ILogger<InvitationFunctions> logger)
        {
            _invitationService = invitationService;
            _userService = userService;
            _roleService = roleService;
            _logger = logger;
        }

        [Function("CreateInvitation")]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseData> CreateInvitation(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "invitation")] HttpRequestData req,
            FunctionContext context)
        {
            var userId = context.GetUserId();
            _logger.LogInformation("Create invitation request by user: {UserId}", userId);

            var dto = await req.ReadFromJsonAsync<CreateInvitationDto>();
            if (dto == null)
            {
                return await req.CreateJsonResponse(HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Invalid request body."));
            }

            var roleExists = await _roleService.RoleExistsAsync(dto.RoleId);
            if (!roleExists) return await req.CreateJsonResponse(HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Invalid role specified."));

            if (await _userService.EmailExistsAsync(dto.Email))
            {
                return await req.CreateJsonResponse(HttpStatusCode.Conflict, ApiResponse<object>.Fail("Email is already registered."));
            }

            var token = await _invitationService.CreateInvitationAsync(dto, userId);
            var inviteUrl = $"{req.Url.Scheme}://{req.Url.Host}/register?token={token}";

            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<object>.Ok(new { InvitationUrl = inviteUrl }));
        }

        [Function("ValidateInvitationToken")]
        public async Task<HttpResponseData> ValidateInvitationToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "invitation/validate")] HttpRequestData req)
        {
            var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var token = queryParams["token"];

            _logger.LogInformation("Validating invitation token.");

            if (string.IsNullOrEmpty(token))
            {
                return await req.CreateJsonResponse(HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Token is required."));
            }

            var invitation = await _invitationService.GetInvitationAsync(token);

            if (invitation == null || invitation.IsUsed || invitation.ExpiresAt < System.DateTime.UtcNow)
            {
                return await req.CreateJsonResponse(HttpStatusCode.NotFound, ApiResponse<object>.NotFound("Invitation token is invalid, used, or expired."));
            }

            var responsePayload = new { Email = invitation.Email, Role = invitation.Role.Type.ToString(), Valid = true };
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<object>.Ok(responsePayload));
        }

        [Function("RegisterWithToken")]
        public async Task<HttpResponseData> RegisterWithToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "invitation/register")] HttpRequestData req)
        {
            _logger.LogInformation("Registration attempt with invitation token.");

            var dto = await req.ReadFromJsonAsync<RegisterWithTokenDto>();
            if (dto == null)
            {
                return await req.CreateJsonResponse(HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Invalid request body."));
            }

            var invitation = await _invitationService.GetInvitationAsync(dto.Token);

            if (invitation == null || invitation.IsUsed || invitation.ExpiresAt < System.DateTime.UtcNow)
            {
                return await req.CreateJsonResponse(HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Invitation token is invalid, used, or expired."));
            }
            if (invitation.Email != dto.Email)
            {
                return await req.CreateJsonResponse(HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Email does not match invitation."));
            }

            var newUser = await _userService.CreateUserAsyncWithInvitation(dto, invitation.RoleId, invitation.ApartmentId);
            invitation.IsUsed = true;
            await _invitationService.UpdateInvitationAsync(invitation);

            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<object>.Ok(new { UserId = newUser.Id }, "User registered successfully."));
        }
    }
}
