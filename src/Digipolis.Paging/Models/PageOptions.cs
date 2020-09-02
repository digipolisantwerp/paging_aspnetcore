using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace Digipolis.Paging.Models
{
    /// <summary>
    /// Class to provide paging functionality. 
    /// Inherit from this class if you need paging.
    /// </summary>
    [Serializable]
    public class PageOptions
    {
        /// <summary>
        /// Page number.
        /// Default value = 1
        /// </summary>
        [JsonProperty("page")]
        [JsonPropertyName("page")]
        [FromQuery(Name = "page")]
        public int Page { get; set; } = Constants.Paging.DefaultPageNumber;

        /// <summary>
        /// Page size.
        /// Default value = 10
        /// </summary>
        [JsonProperty("pagesize")]
        [JsonPropertyName("pagesize")]
        [FromQuery(Name = "pagesize")]
        public int PageSize { get; set; } = Constants.Paging.DefaultPageSize;

        /// <summary>
        /// Option to enable/disable requesting a total result count.
        /// Current values: WithCount or NoCount
        /// Default value = WithCount
        /// </summary>
        [JsonProperty("paging-strategy")]
        [JsonPropertyName("paging-strategy")]
        [FromQuery(Name = "paging-strategy")]
        public PagingStrategy? PagingStrategy { get; set; } = Models.PagingStrategy.WithCount;
    }
}
