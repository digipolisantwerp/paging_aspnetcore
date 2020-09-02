using Digipolis.Paging.Models;
using System.Collections.Generic;

namespace Digipolis.Paging.Mapping
{
    public interface IPageConverter
    {
        /// <summary>
        /// Convert a DataPage to a paged result 
        /// </summary>
        /// <param name="pageSortOptions">Options for paging and sorting</param>
        /// <param name="dataPage">List of data objects of type T with paging information</param>
        /// <returns>A paged result based on the HAL specification</returns>
        PagedResult<T, TEmbedded> ToPagedResult<T, TEmbedded>(PageSortOptions pageSortOptions, DataPage<T> dataPage)
           where TEmbedded : IEmbedded<T>, new()
           where T : class;

        /// <summary>
        /// Convert a DataPage to a paged result 
        /// </summary>
        /// <param name="pageOptions">Options for paging</param>
        /// <param name="dataPage">List of data objects of type T with paging information</param>
        /// <returns>A paged result based on the HAL specification</returns>
        PagedResult<T, TEmbedded> ToPagedResult<T, TEmbedded>(PageOptions pageOptions, DataPage<T> dataPage)
            where TEmbedded : IEmbedded<T>, new()
            where T : class;

        /// <summary>
        /// Convert data to a paged result 
        /// </summary>
        /// <param name="pageSortOptions">Options for paging and sorting</param>
        /// <param name="data">IEnumerable of data objects of type T</param>
        /// <param name="total">Total result count</param>
        /// <returns>A paged result based on the HAL specification</returns>
        PagedResult<T, TEmbedded> ToPagedResult<T, TEmbedded>(PageSortOptions pageSortOptions, IEnumerable<T> data, long? total)
            where TEmbedded : IEmbedded<T>, new()
            where T : class;

        /// <summary>
        /// Convert data to a paged result 
        /// </summary>
        /// <param name="pageOptions">Options for paging</param>
        /// <param name="data">IEnumerable of data objects of type T</param>
        /// <param name="total">Total result count</param>
        /// <returns>A paged result based on the HAL specification</returns>
        PagedResult<T, TEmbedded> ToPagedResult<T, TEmbedded>(PageOptions pageOptions, IEnumerable<T> data, long? total)
            where TEmbedded : IEmbedded<T>, new()
            where T : class;
    }
}