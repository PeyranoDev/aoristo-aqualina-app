using AoristoTowersFunctions.Helpers;
using AoristoTowersFunctions.Middleware;
using Common.Models.Requests;
using Common.Models.Responses;
using Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Services.Main.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace AoristoTowersFunctions.Functions
{
    public class ApartmentFunctions
    {
        private readonly IApartmentService _apartmentService;
        private readonly ILogger<ApartmentFunctions> _logger;

        public ApartmentFunctions(IApartmentService apartmentService, ILogger<ApartmentFunctions> logger)
        {
            _apartmentService = apartmentService;
            _logger = logger;
        }

        [Function("CreateApartment")]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseData> CreateApartment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "apartment")] HttpRequestData req)
        {
            _logger.LogInformation("Create apartment request received.");
            var dto = await req.ReadFromJsonAsync<ApartmentForCreateDTO>();
            if (dto == null)
            {
                return await req.CreateJsonResponse(HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Invalid request body."));
            }
            var apartment = await _apartmentService.CreateApartmentAsync(dto);
            return await req.CreateJsonResponse(HttpStatusCode.Created, ApiResponse<Apartment>.Ok(apartment));
        }

        [Function("GetAllApartments")]
        [Authorize(Roles = "Admin,Security")]
        public async Task<HttpResponseData> GetAllApartments(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "apartment")] HttpRequestData req)
        {
            _logger.LogInformation("Get all apartments request received.");
            var apartments = await _apartmentService.GetAllApartmentsAsync();
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<List<Apartment>>.Ok(apartments));
        }

        [Function("GetApartmentById")]
        [Authorize(Roles = "Admin,Security")]
        public async Task<HttpResponseData> GetApartmentById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "apartment/{apartmentId:int}")] HttpRequestData req, int apartmentId)
        {
            _logger.LogInformation("Get apartment by ID: {ApartmentId}", apartmentId);
            var apartment = await _apartmentService.GetApartmentByIdAsync(apartmentId);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<Apartment>.Ok(apartment));
        }

        [Function("GetApartmentByUserId")]
        [Authorize(Roles = "Admin,Security")]
        public async Task<HttpResponseData> GetApartmentByUserId(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "apartment/user/{userId:int}")] HttpRequestData req, int userId)
        {
            _logger.LogInformation("Get apartment by user ID: {UserId}", userId);
            var apartment = await _apartmentService.GetApartmentsByUserIdAsync(userId);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<Apartment>.Ok(apartment));
        }

        [Function("UpdateApartment")]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseData> UpdateApartment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "apartment/{apartmentId:int}")] HttpRequestData req, int apartmentId)
        {
            _logger.LogInformation("Update apartment request for ID: {ApartmentId}", apartmentId);
            var apartmentDto = await req.ReadFromJsonAsync<Apartment>();
            if (apartmentDto == null || apartmentDto.Id != apartmentId)
            {
                return await req.CreateJsonResponse(HttpStatusCode.BadRequest, ApiResponse<object>.Fail("Invalid request body or ID mismatch."));
            }
            var updatedApartment = await _apartmentService.UpdateApartmentAsync(apartmentDto);
            return await req.CreateJsonResponse(HttpStatusCode.OK, ApiResponse<Apartment>.Ok(updatedApartment));
        }

        [Function("DeleteApartment")]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseData> DeleteApartment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "apartment/{apartmentId:int}")] HttpRequestData req, int apartmentId)
        {
            _logger.LogInformation("Delete apartment request for ID: {ApartmentId}", apartmentId);
            await _apartmentService.DeleteApartmentAsync(apartmentId);
            return req.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
