using Data.Entities;
using System.Linq;
using System.Linq.Expressions;

namespace Common.Helpers
{
    public static class VehicleSortingHelper
    {
        public static IQueryable<Vehicle> ApplySorting(
            this IQueryable<Vehicle> source,
            string sortBy,
            string sortOrder)
        {
            return sortOrder?.ToLower() == "desc"
                ? source.OrderByDescending(GetSortProperty(sortBy))
                : source.OrderBy(GetSortProperty(sortBy));
        }

        private static Expression<Func<Vehicle, object>> GetSortProperty(string propertyName)
        {
            return propertyName?.ToLower() switch
            {
                "plate" => v => v.Plate,
                "model" => v => v.Model,
                "color" => v => v.Color,
                "isparked" => v => v.IsParked,
                "isactive" => v => v.IsActive,
                _ => v => v.Id
            };
        }

        public static IQueryable<Vehicle> ApplyFilters(
            this IQueryable<Vehicle> query,
            string? plate,
            string? model,
            bool? isActive,
            bool? hasRequests)
        {
            if (!string.IsNullOrEmpty(plate))
                query = query.Where(v => v.Plate.Contains(plate));

            if (!string.IsNullOrEmpty(model))
                query = query.Where(v => v.Model.Contains(model));

            if (isActive.HasValue)
                query = query.Where(v => v.IsActive == isActive.Value);

            if (hasRequests.HasValue)
                query = hasRequests.Value
                    ? query.Where(v => v.Requests.Any())
                    : query.Where(v => !v.Requests.Any());

            return query;
        }
    }
}