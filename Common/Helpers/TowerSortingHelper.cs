using Common.Models.Requests;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class TowerQueryHelper
    {
        /// <summary>
        /// Aplica el ordenamiento dinámico a una consulta de Torres.
        /// </summary>
        public static IQueryable<Tower> ApplySorting(
            this IQueryable<Tower> source,
            string sortBy,
            string sortOrder)
        {
            // Si no se especifica sortBy, no se puede ordenar.
            if (string.IsNullOrWhiteSpace(sortBy)) return source;

            return sortOrder.ToLower() == "desc"
                ? source.OrderByDescending(GetSortProperty(sortBy))
                : source.OrderBy(GetSortProperty(sortBy));
        }

        /// <summary>
        /// Devuelve la expresión de propiedad para el ordenamiento basado en un string.
        /// </summary>
        private static Expression<Func<Tower, object>> GetSortProperty(string propertyName)
        {
            return propertyName.ToLower() switch
            {
                "adress" => tower => tower.Adress,
                // Por defecto, o si se especifica "name"
                _ => tower => tower.Name
            };
        }

        /// <summary>
        /// Aplica los filtros dinámicos a una consulta de Torres.
        /// </summary>
        public static IQueryable<Tower> ApplyFilters(this IQueryable<Tower> query, TowerFilterParams filters)
        {
            if (filters == null) return query;

            if (!string.IsNullOrEmpty(filters.Name))
            {
                query = query.Where(t => t.Name.ToLower().Contains(filters.Name.ToLower()));
            }

            return query;
        }
    }
}
