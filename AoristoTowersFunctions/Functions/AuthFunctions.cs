using AoristoTowersFunctions.Helpers;
using AoristoTowersFunctions.Middleware;
using Common.Models.Requests;
using Common.Models.Responses;
using Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Services.Main.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AoristoTowersFunctions.Functions
{
    public class AuthFunctions
    {
        private readonly ILogger<AuthFunctions> _logger;
        private readonly IUserService _userService;
        private readonly JwtOptions _jwtOptions;

        public AuthFunctions(ILogger<AuthFunctions> logger, IUserService userService, JwtOptions jwtOptions)
        {
            _logger = logger;
            _userService = userService;
            _jwtOptions = jwtOptions;
        }

        [Function("Login")]
        public async Task<HttpResponseData> Login(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/login")] HttpRequestData req)
        {
            var credentials = await req.ReadFromJsonAsync<CredentialsDTO>();

            if (credentials == null)
            {
                return await req.CreateJsonResponse(HttpStatusCode.BadRequest,
                    ApiResponse<object>.Fail("Invalid request body."));
            }

            User? user = await _userService.ValidateAsync(credentials);

            if (user is null ||
                !user.GetAssociatedTowerIds().Contains(credentials.SelectedTowerId))
            {
                return await req.CreateJsonResponse(HttpStatusCode.Forbidden,
                    ApiResponse<object>.Fail("Invalid credentials or tower access."));
            }

            var authResponse = GenerateJwt(user, credentials.SelectedTowerId);
            return await req.CreateJsonResponse(HttpStatusCode.OK,
                ApiResponse<AuthResponseDto>.Ok(authResponse, "Login successful."));
        }

        [Function("Register")]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseData> Register(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/register")] HttpRequestData req)
        {
            _logger.LogInformation("Register function processed a request.");

            var createDto = await req.ReadFromJsonAsync<UserForCreateDTO>();
            if (createDto == null)
            {
                return await req.CreateJsonResponse(HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Invalid request body."));
            }

            var userResponse = await _userService.CreateUserAsync(createDto);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<UserForResponse>.Ok(userResponse, "User registered successfully."));
        }

        private AuthResponseDto GenerateJwt(User user, int selectedTowerId)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new("username", user.Username),
                new("fullName", $"{user.Name} {user.Surname}"),
                new(ClaimTypes.Role, user.Role.Type.ToString()),
                new("towerId", selectedTowerId.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return new AuthResponseDto { AccessToken = jwt };
        }
    }
}