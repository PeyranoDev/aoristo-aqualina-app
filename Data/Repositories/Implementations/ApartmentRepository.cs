using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Implementations
{
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly AqualinaAPIContext _context;

        public ApartmentRepository(AqualinaAPIContext context)
        {
            _context = context;
        }

        public async Task<List<Apartment>> GetApartmentsAsync()
        {
            return await _context.Apartments.ToListAsync();
        }
        public async Task<Apartment?> GetByIdAsync(int id)
        {
            return await _context.Apartments
                .Include(a => a.Users) // Include related users
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Apartment> CreateAsync(Apartment apartment)
        {
            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();
            return apartment;
        }
        public async Task<Apartment?> UpdateAsync(Apartment apartment)
        {
            var existingApartment = await _context.Apartments.FindAsync(apartment.Id);
            if (existingApartment == null) return null;

            _context.Entry(existingApartment).CurrentValues.SetValues(apartment);

            await _context.SaveChangesAsync();
            return existingApartment;
        }
        public async Task<bool> IdentifierExistsAsync(string identifier)
        {
            return await _context.Apartments.AnyAsync(a => a.Identifier == identifier);
        }

        public async Task<bool> IsApartmentActiveAsync(int id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            return apartment != null && apartment.IsActive;
        }

        public async Task<List<Apartment>> GetActiveApartmentsAsync()
        {
            return await _context.Apartments
                .Where(a => a.IsActive)
                .ToListAsync();
        }
    }
}
