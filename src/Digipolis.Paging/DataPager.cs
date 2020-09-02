using Digipolis.Paging.Extensions;
using Digipolis.Paging.Models;
using Digipolis.Paging.Predicates;
using System.Linq;
using System.Threading.Tasks;

namespace Digipolis.Paging
{
    /// <summary>
    /// Execute queries with paging and sorting
    /// </summary>
    public class DataPager : IDataPager
    {

        public DataPager()
        {
        }

        /// <summary>
        /// Execute a query with paging and sorting asynchronously
        /// </summary>
        /// <param name="query">The given IQueryable</param>
        /// <param name="pageSortOptions">Options for paging and sorting</param>
        /// <returns>A data page containing the result set and paging information</returns>
        public async Task<DataPage<T>> Page<T>(
            IQueryable<T> query,
            PageSortOptions pageSortOptions) where T : class
        {
            int totalCount = 0;
            if (pageSortOptions?.PagingStrategy == null || pageSortOptions.PagingStrategy == PagingStrategy.WithCount)
            {
                totalCount = await query.CountAsyncSafe();
            }

            var results = await query
                .OrderBy(pageSortOptions?.Sort)
                .CreatePage(pageSortOptions?.Page ?? Constants.Paging.DefaultPageNumber, pageSortOptions?.PageSize ?? Constants.Paging.DefaultPageSize)
                .ToListAsyncSafe();

            return new DataPage<T>(results, totalCount, pageSortOptions?.Page ?? Constants.Paging.DefaultPageNumber, pageSortOptions?.PageSize ?? Constants.Paging.DefaultPageSize);
        }

        /// <summary>
        /// Execute a query with paging and sorting asynchronously
        /// </summary>
        /// <param name="query">The given IQueryable</param>
        /// <param name="pageOptions">Options for paging</param>
        /// <returns>A data page containing the result set and paging information</returns>
        public async Task<DataPage<T>> Page<T>(
           IQueryable<T> query,
           PageOptions pageOptions)
            where T : class
        {
            int totalCount = 0;
            if (pageOptions?.PagingStrategy == null || pageOptions.PagingStrategy == PagingStrategy.WithCount)
            {
                totalCount = await query.CountAsyncSafe();
            }

            var results = await query
                .CreatePage(pageOptions?.Page ?? Constants.Paging.DefaultPageNumber, pageOptions?.PageSize ?? Constants.Paging.DefaultPageSize)
                .ToListAsyncSafe();

            return new DataPage<T>(results, totalCount, pageOptions?.Page ?? Constants.Paging.DefaultPageNumber, pageOptions?.PageSize ?? Constants.Paging.DefaultPageSize);
        }
    }
}
