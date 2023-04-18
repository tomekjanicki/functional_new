using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ApiClient.Infrastructure;
using ApiClient.V2.Services.Abstractions;
using OneOf;

namespace ApiClient.V2.Services
{
    public sealed class EventApi : IEventApi
    {
        private readonly HttpClient _httpClient;

        internal EventApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OneOf<string, Error>> Create(string body, CancellationToken token = default)
        {
            using var inputContent = body.GetStringInputContent();
            using var response = await _httpClient.PostAsync("event", content: inputContent, token: token).ConfigureAwait(false);

            return await response.Content.HandleResultOrError<string>(response.StatusCode, HttpStatusCode.Created).ConfigureAwait(false);
        }
    }
}