using Common.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class SortingHelper
    {
        public static IQueryable<User> ApplySorting(
            this IQueryable<User> source,
            string sortBy,
            string sortOrder)
        {
            return sortOrder.ToLower() == "desc"
                ? source.OrderByDescending(GetSortProperty(sortBy))
                : source.OrderBy(GetSortProperty(sortBy));
        }

        private static Expression<Func<User, object>> GetSortProperty(string propertyName)
        {
            return propertyName.ToLower() switch
            {
                "email" => x => x.Email,
                "surname" => x => x.Surname,
                "role" => x => x.Role.Type,
                _ => x => x.Name
            };
        }
        public static IQueryable<User> ApplyFilters(this IQueryable<User> query, UserFilterParams filters)
        {
            if (filters == null) return query;

            // Aplicar filtros individuales
            if (!string.IsNullOrEmpty(filters.Name))
                query = query.Where(u => u.Name.Contains(filters.Name));


            if (!string.IsNullOrEmpty(filters.Email))
                query = query.Where(u => u.Email == filters.Email);

            if (!string.IsNullOrEmpty(filters.RoleType))
                query = query.Where(u => u.Role.Type.ToString() == filters.RoleType);

            if (!string.IsNullOrEmpty(filters.ApartmentIdentifier))
                query = query.Where(u => u.Apartment != null && u.Apartment.Identifier == filters.ApartmentIdentifier);

            if (filters.IsActive.HasValue)
                query = query.Where(u => u.IsActive == filters.IsActive.Value);

            return query;
        }
    }
}

