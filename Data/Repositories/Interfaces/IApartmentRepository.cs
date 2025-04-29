using Data.Entities;

namespace Data.Repositories.Interfaces
{
    public interface IApartmentRepository
    {
        Task<Apartment> CreateAsync(Apartment apartment);
        Task<List<Apartment>> GetActiveApartmentsAsync();
        Task<List<Apartment>> GetApartmentsAsync();
        Task<Apartment?> GetByIdAsync(int id);
        Task<bool> IdentifierExistsAsync(string identifier);
        Task<bool> IsApartmentActiveAsync(int id);
        Task<Apartment?> UpdateAsync(Apartment apartment);
    }
}