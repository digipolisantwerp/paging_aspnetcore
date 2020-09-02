using System.Collections.Generic;

namespace Digipolis.Paging.Models
{
    /// <summary>
    /// Implement this interface, providing a type for T, to use as TEmbedded for <see cref="PagedResult{T, TEmbedded}"/>
    /// In your implementation, set a custom json property name for the resourceList by using the JsonPropertyName attribute (from System.Text.Json) 
    /// or JsonProperty attribute (from Newtonsoft.Json) depending on your serializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEmbedded<T>
        where T : class
    {
        /// <summary>
        /// Requested data.
        /// </summary>
        IEnumerable<T> ResourceList { get; set; }
    }
}

