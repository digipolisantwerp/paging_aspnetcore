using System;
using System.Collections.Generic;

namespace Digipolis.Paging.Models
{
    /// <summary>
    /// List of data objects of type T with paging information
    /// </summary>
    [Serializable]
    public class DataPage<T> where T : class
    {
        /// <summary>
        /// List of data objects of type T
        /// </summary>
        public List<T> Data { get; set; }
        /// <summary>
        /// Total result count
        /// </summary>
        public long TotalEntityCount { get; set; }
        /// <summary>
        /// Page number
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// Page size
        /// </summary>
        public int PageLength { get; set; }
        /// <summary>
        /// Total page count
        /// </summary>
        public int TotalPageCount { get; }

        public DataPage(List<T> data, int totalEntityCount, int? pageNumber = null, int? pageLength = null)
        {
            if (!pageNumber.HasValue)
                pageNumber = Constants.Paging.DefaultPageNumber;
            if (!pageLength.HasValue)
                pageLength = Constants.Paging.DefaultPageSize;

            Data = data;
            TotalEntityCount = totalEntityCount;
            PageNumber = pageNumber.Value;
            PageLength = data?.Count ?? 0;
            TotalPageCount = pageLength < 1 ? 0 : (int)Math.Ceiling((double)totalEntityCount / pageLength.Value);
        }
    }
}
