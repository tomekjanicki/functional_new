using System.Collections.Generic;

namespace ApiClient.V2.Models.Abstractions
{
    public interface IWithCustomFragments
    {
        IReadOnlyDictionary<string, object?> CustomFragments { get; set; }
    }
}