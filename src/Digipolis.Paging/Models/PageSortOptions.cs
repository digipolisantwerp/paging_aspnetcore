using Digipolis.Paging.Constants;
using System;

namespace Digipolis.Paging.Models
{
    /// <summary>
    /// Class to provide paging and sorting functionality. 
    /// Inherit from this class if you need paging and sorting.
    /// </summary>
    [Serializable]
    public class PageSortOptions : PageOptions
    {
        /// <summary>
        /// Property name or comma seperated list of the names of the properties to sort by. 
        /// In order to sort descending, set '-' in front of the property name (e.g. "-id"). 
        /// Default value = "id" if an Id property exists. In other cases the first property defind in the class will be used as a fallback.
        /// </summary>
        public string Sort { get; set; } = Sorting.Default;
    }
}
