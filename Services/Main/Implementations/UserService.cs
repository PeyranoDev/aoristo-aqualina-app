using AutoMapper;
using Common.Models;
using Data.Entities;
using Data.Repositories.Interfaces;
using Services.Main.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Services.Main.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IHashingService _hashingService;
        private readonly IMapper _mapper;
        private readonly IApartmentRepository _apartmentRepo; 

        public UserService(
            IUserRepository userRepo,
            IHashingService hashingService,
            IMapper mapper,
            IApartmentRepository apartmentRepo = null)
        {
            _userRepo = userRepo;
            _hashingService = hashingService;
            _mapper = mapper;
            _apartmentRepo = apartmentRepo;
        }

        public async Task<User?> ValidateAsync(CreedentialsDTO dto)
        {
            var hashedPassword = _hashingService.HashPassword(dto.Password);
            return await _userRepo.ValidateAsync(dto.Username, hashedPassword);
        }

        public async Task<User?> UpdateUserAsync(UserForUpdateDTO dto, int userId)
        {
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true))
            {
                throw new ValidationException(validationResults.First().ErrorMessage);
            }

            var existingUser = await _userRepo.GetByIdAsync(userId);
            if (existingUser == null) return null;

            _mapper.Map(dto, existingUser);

            if (dto.Apartment_Id.HasValue && _apartmentRepo != null)
            {
                var apartment = await _apartmentRepo.GetByIdAsync(dto.Apartment_Id.Value);
                existingUser.Apartment = apartment;
            }

            return await _userRepo.UpdateAsync(existingUser);
        }
    }
}