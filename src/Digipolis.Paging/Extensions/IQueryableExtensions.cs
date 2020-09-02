using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digipolis.Paging.Extensions
{
    public static class IQueryableExtensions
    {
        public static Task<List<T>> ToListAsyncSafe<T>(this IQueryable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source is IAsyncEnumerable<T>))
                return Task.FromResult(source.ToList());
            return source.ToListAsync();
        }

        public static Task<int> CountAsyncSafe<T>(this IQueryable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source is IAsyncEnumerable<T>))
                return Task.FromResult(source.Count());
            return source.CountAsync();
        }
    }
}
