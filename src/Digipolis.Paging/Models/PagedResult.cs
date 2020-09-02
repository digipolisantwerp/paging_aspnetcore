using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Digipolis.Paging.Models
{
    /// <summary>
    /// Using this PagedResult, serialization will generate a json string where the _embedded HAL object an object will have a property with a name 
    /// that can be set with the JsonPropertyAttribute on the overridden ResourceList property of the inherited <see cref="IEmbedded{T}"/> that is used as TEmbedded.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TEmbedded">The type of the embedded data.</typeparam>
    [Serializable]
    public class PagedResult<T, TEmbedded>
        where T : class
        where TEmbedded : IEmbedded<T>, new()
    {
        /// <summary>
        /// Contains links to other pages of the result set.
        /// </summary>
        [JsonProperty("_links")]
        [JsonPropertyName("_links")]
        public PagedResultLinks Links { get; set; }

        /// <summary>
        /// Wraps the the result set.
        /// </summary>
        [JsonProperty("_embedded")]
        [JsonPropertyName("_embedded")]
        public TEmbedded Embedded { get; set; }

        /// <summary>
        /// Paging information of the result set.
        /// </summary>
        [JsonProperty("_page")]
        [JsonPropertyName("_page")]
        public Page Page { get; set; }

        public PagedResult()
        {

        }

        public PagedResult(int page, int pageSize, long? totalElements, IEnumerable<T> data)
        {
            Links = new PagedResultLinks();
            Embedded = new TEmbedded { ResourceList = data ?? new List<T>() };
            Page = new Page
            {
                Number = page,
                Size = data?.Count() ?? 0,
                TotalElements = totalElements,
                TotalPages = pageSize == 0 ? 0 : (int)Math.Ceiling((double)totalElements / pageSize)
            };
        }
    }
}

