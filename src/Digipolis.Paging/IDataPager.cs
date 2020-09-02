using Digipolis.Paging.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Digipolis.Paging
{
    /// <summary>
    /// Execute queries with paging and sorting
    /// </summary>
    public interface IDataPager
    {
        /// <summary>
        /// Execute a query with paging and sorting
        /// </summary>
        /// <param name="query">The given IQueryable</param>
        /// <param name="pageSortOptions">Options for paging and sorting</param>
        /// <returns>A data page containing the result set and paging information</returns>
        Task<DataPage<T>> Page<T>(IQueryable<T> query, PageSortOptions pageSortOptions)
            where T : class;

        /// <summary>
        /// Execute a query with paging and sorting
        /// </summary>
        /// <param name="query">The given IQueryable</param>
        /// <param name="pageOptions">Options for paging</param>
        /// <returns>A data page containing the result set and paging information</returns>
        Task<DataPage<T>> Page<T>(IQueryable<T> query, PageOptions pageOptions) 
            where T : class;
    }
}