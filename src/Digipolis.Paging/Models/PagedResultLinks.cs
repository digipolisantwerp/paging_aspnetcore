using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace Digipolis.Paging.Models
{
    [Serializable]
    public class PagedResultLinks
    {
        /// <summary>
        /// Link to the first page of the result set.
        /// </summary>
        [JsonProperty("first")]
        [JsonPropertyName("first")]
        public Link First { get; set; }

        /// <summary>
        /// Link to the previous page of the result set.
        /// </summary>
        [JsonProperty("prev")]
        [JsonPropertyName("prev")]
        public Link Previous { get; set; }

        /// <summary>
        /// Link to the current page of the result set.
        /// </summary>
        [JsonProperty("self")]
        [JsonPropertyName("self")]
        public Link Self { get; set; }

        /// <summary>
        /// Link to the next page of the result set.
        /// </summary>
        [JsonProperty("next")]
        [JsonPropertyName("next")]
        public Link Next { get; set; }

        /// <summary>
        /// Link to the last page of the result set.
        /// </summary>
        [JsonProperty("last")]
        [JsonPropertyName("last")]
        public Link Last { get; set; }
    }
}