using Common.Exceptions;
using Common.Models.Requests;
using Data.Entities;
using Data.Repositories.Interfaces;
using Services.Main.Interfaces;

namespace Services.Main.Implementations
{
    public class ApartmentService : IApartmentService
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IUserRepository _userRepository;

        public ApartmentService(IApartmentRepository apartmentRepository, IUserRepository userRepository)
        {
            _apartmentRepository = apartmentRepository;
            _userRepository = userRepository;
        }

        public async Task<Apartment> CreateApartmentAsync(ApartmentForCreateDTO apartment)
        {
            if (await _apartmentRepository.IdentifierExistsAsync(apartment.Identifier))
                throw new ApartmentConflictException(apartment.Identifier);

            var newApartment = new Apartment
            {
                Identifier = apartment.Identifier,
            };

            return await _apartmentRepository.CreateAsync(newApartment);
        }

        public async Task<List<Apartment>> GetAllApartmentsAsync()
        {
            return await _apartmentRepository.GetApartmentsAsync();
        }

        public async Task<Apartment?> GetApartmentByIdAsync(int id)
        {
            var apartment = await _apartmentRepository.GetByIdAsync(id);
            if (apartment == null)
                throw new ApartmentNotFoundException(id);

            return apartment;
        }

        public async Task<Apartment?> UpdateApartmentAsync(Apartment apartment)
        {
            if (apartment.Id <= 0)
                throw new BadRequestException("Invalid apartment ID.");

            var existingApartment = await _apartmentRepository.GetByIdAsync(apartment.Id);
            if (existingApartment == null)
                throw new ApartmentNotFoundException(apartment.Id);

            return await _apartmentRepository.UpdateAsync(apartment);
        }

        public async Task DeleteApartmentAsync(int id)
        {
            var apartment = await _apartmentRepository.GetByIdAsync(id);
            if (apartment == null)
                throw new ApartmentNotFoundException(id);
            try
            {
                await _apartmentRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo eliminar el departamento", ex);
            }
        }

        public async Task<Apartment> GetApartmentsByUserIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var apartment = user.Apartment;
            if (apartment == null)
                throw new UserWithoutApartmentException(userId);

            return apartment;
        }
    }
}
