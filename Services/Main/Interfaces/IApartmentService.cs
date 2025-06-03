using Common.Models.Requests;
using Data.Entities;

namespace Services.Main.Interfaces
{
    public interface IApartmentService
    {
        Task<Apartment> CreateApartmentAsync(ApartmentForCreateDTO apartment);
        Task<List<Apartment>> GetAllApartmentsAsync();
        Task<Apartment?> GetApartmentByIdAsync(int id);
        Task<Apartment?> UpdateApartmentAsync(Apartment apartment);
    }
}