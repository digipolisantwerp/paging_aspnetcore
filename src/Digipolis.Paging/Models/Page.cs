using System;

namespace Digipolis.Paging.Models
{
    /// <summary>
    /// Paging information object.
    /// </summary>
    [Serializable]
    public class Page
    {
        /// <summary>
        /// Page number
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Page size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Total result count
        /// </summary>
        public long? TotalElements { get; set; }

        /// <summary>
        /// Total page count
        /// </summary>
        public int? TotalPages { get; set; }
    }
}
