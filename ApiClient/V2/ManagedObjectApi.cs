using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ApiClient.Infrastructure;
using ApiClient.V2.Abstractions;
using OneOf;

namespace ApiClient.V2
{
    public sealed class ManagedObjectApi : IManagedObjectApi
    {
        private readonly HttpClient _httpClient;

        internal ManagedObjectApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OneOf<IEnumerable<string>, Error>> GetItems(CancellationToken token = default)
        {
            using var response = await _httpClient.GetAsync("managedObject", token: token).ConfigureAwait(false);

            return await response.Content.HandleResultOrError<IEnumerable<string>>(response.StatusCode, HttpStatusCode.OK).ConfigureAwait(false);
        }
    }
}