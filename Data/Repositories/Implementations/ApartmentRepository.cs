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
                .Include(a => a.Users) 
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

        public async Task<bool> DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await DetachUsersFromApartmentAsync(id);

                var apartment = await _context.Apartments.FindAsync(id);
                if (apartment == null) return false;

                _context.Apartments.Remove(apartment);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task DetachUsersFromApartmentAsync(int apartmentId)
        {
            var users = await _context.Users
                .Where(u => u.ApartmentId == apartmentId)
                .ToListAsync();

            foreach (var user in users)
            {
                user.ApartmentId = null;
                user.Apartment = null;
            }

            await _context.SaveChangesAsync();
        }

    }
}
