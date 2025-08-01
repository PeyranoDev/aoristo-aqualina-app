﻿using Azure.Security.KeyVault.Secrets;
using Common.Models;
using Common.Models.Requests;
using Common.Models.Responses;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services.Main.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace aoristo_aqualina_app.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly JwtOptions _jwtOptions;

        public AuthController(JwtOptions jwtOptions, IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
            _jwtOptions = jwtOptions;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Auth([FromBody] CredentialsDTO dto)
        {
            User? user = await _userService.ValidateAsync(dto);

            if (user is null)
            {
                return Forbid();
            }
            var associatedTowerIds = user.GetAssociatedTowerIds();

           
            if (!associatedTowerIds.Contains(dto.SelectedTowerId))
            {
                return Forbid();
            }

            var authResponse = GenerateFinalJwtResponse(user, dto.SelectedTowerId);

            return Ok(ApiResponse<AuthResponseDto>.Ok(authResponse, "User logged in successfully."));
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] UserForCreateDTO dto)
        {
            if (await _userService.EmailExistsAsync(dto.Email))
                return BadRequest("Email already exists.");
            if (await _userService.UsernameExistsAsync(dto.Username))
                return BadRequest("Username already exists.");

            var userResponse = await _userService.CreateUserAsync(dto);
            return Ok(ApiResponse<UserForResponse>.Ok(userResponse, "User registered successfully."));
        }

        private AuthResponseDto GenerateFinalJwtResponse(User user, int selectedTowerId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim("full name", $"{user.Name} {user.Surname}"),
                new Claim(ClaimTypes.Role, user.Role.Type.ToString()),
                new Claim("towerId", selectedTowerId.ToString()) 
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30), 
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new AuthResponseDto
            {
                AccessToken = jwt,
            };
        }
    }
}
