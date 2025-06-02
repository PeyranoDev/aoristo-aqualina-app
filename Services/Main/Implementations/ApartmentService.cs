using Common.Models.Requests;
using Data.Entities;
using Data.Repositories.Interfaces;
using Services.Main.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Main.Implementations
{
    public class ApartmentService : IApartmentService
    {
        private readonly IApartmentRepository _apartmentRepository;

        public ApartmentService(IApartmentRepository apartmentRepository)
        {
            _apartmentRepository = apartmentRepository;
        }

        public async Task<Apartment> CreateApartmentAsync(ApartmentForCreateDTO apartment)
        {
            if (await _apartmentRepository.IdentifierExistsAsync(apartment.Identifier))
            {
                throw new InvalidOperationException("An apartment with this identifier already exists.");
            }

            var newApartment = new Apartment
            {
                Identifier = apartment.Identifier,
                IsActive = true
            };

            return await _apartmentRepository.CreateAsync(newApartment);

        }

        public async Task<List<Apartment>> GetAllApartmentsAsync()
        {
            return await _apartmentRepository.GetApartmentsAsync();
        }

        public async Task<Apartment?> GetApartmentByIdAsync(int id)
        {
            return await _apartmentRepository.GetByIdAsync(id);
        }
        public async Task<Apartment?> UpdateApartmentAsync(Apartment apartment)
        {
            if (apartment.Id <= 0)
            {
                throw new ArgumentException("Invalid apartment ID.");
            }

            var existingApartment = await _apartmentRepository.GetByIdAsync(apartment.Id);
            if (existingApartment == null)
            {
                throw new KeyNotFoundException("Apartment not found.");
            }

            return await _apartmentRepository.UpdateAsync(apartment);
        }


    }
}
