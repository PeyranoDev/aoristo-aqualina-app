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
    public class NotificationFunctions
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<NotificationFunctions> _logger;

        public NotificationFunctions(ITokenService tokenService, ILogger<NotificationFunctions> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        [Function("RegisterPushToken")]
        [Authorize]
        public async Task<HttpResponseData> RegisterPushToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "notification/register")] HttpRequestData req,
            FunctionContext context)
        {
            var userId = context.GetUserId();
            _logger.LogInformation("Registering push token for user ID: {UserId}", userId);

            var dto = await req.ReadFromJsonAsync<NotificationTokenCreateDTO>();
            if (dto == null || string.IsNullOrEmpty(dto.Token))
            {
                return await req.CreateJsonResponse(HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Token cannot be null or empty."));
            }

            var success = await _tokenService.AddNotificationTokenAsync(dto, userId);

            if (success)
            {
                return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<object>.Ok("Token registered successfully."));
            }

            return await req.CreateJsonResponse(HttpStatusCode.InternalServerError, ApiResponse<object>.Fail("Failed to register token."));
        }
    }
}
