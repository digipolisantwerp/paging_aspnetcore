using System;

namespace Digipolis.Paging.Models
{
    /// <summary>
    /// Option to enable/disable requesting a total result count.
    /// Current values: WithCount or NoCount
    /// </summary>
    [Serializable]
    public enum PagingStrategy
    {
        WithCount,
        NoCount
    }
}
