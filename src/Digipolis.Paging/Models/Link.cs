using System;

namespace Digipolis.Paging.Models
{
    /// <summary>
    /// Link information.
    /// </summary>
    [Serializable]
    public class Link
    {
        /// <summary>
        /// Contains a uri
        /// </summary>
        public string Href { get; set; }

        public Link()
        { }

        public Link(string href)
        {
            Href = href;
        }
    }
}