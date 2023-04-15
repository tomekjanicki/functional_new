using System.Threading;
using System.Threading.Tasks;
using ApiClient.Infrastructure;
using OneOf;

namespace ApiClient.V2.Abstractions
{
    public interface IEventApi
    {
        Task<OneOf<string, Error>> Create(string body, CancellationToken token = default);
    }
}