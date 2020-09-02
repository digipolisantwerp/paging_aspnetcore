using System;
using System.Linq;

namespace Digipolis.Paging.Predicates
{
    /// <summary>
    /// IQueryable extension methods for paging
    /// </summary>
    public static class PagePredicate
    {
        /// <summary>
        /// Create a page of an IQueryable.
        /// </summary>
        /// <param name="query">The given IQueryable.</param>
        /// <param name="pageNumber">Requested page number. Default value = 1</param>
        /// <param name="pageSize">Requested page size. Default value = 10</param>
        /// <returns>A new IQueryable.</returns>
        public static IQueryable<T> CreatePage<T>(this IQueryable<T> query, int? pageNumber = 1, int? pageSize = 10) where T : class
        {
            if (pageNumber == null || pageNumber < 1)
                throw new ArgumentException("Page number cannot be 0 or negative");
            if (pageSize == null || pageSize < 1)
                throw new ArgumentException("Page size cannot be 0 or negative");
            return query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }
    }
}
