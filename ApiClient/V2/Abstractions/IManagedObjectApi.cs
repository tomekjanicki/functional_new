using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiClient.Infrastructure;
using OneOf;

namespace ApiClient.V2.Abstractions
{
    public interface IManagedObjectApi
    {
        Task<OneOf<IEnumerable<string>, Error>> GetItems(CancellationToken token = default);
    }
}