using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ApiClient.Models;
using ApiClient.Models.Dtos;
using ApiClient.Services.Abstractions;
using OneOf;
using OneOf.Types;
using Error = ApiClient.Infrastructure.Error;

namespace ApiClient.Services
{
    public sealed class Client : IClient
    {
        private readonly HttpClient _httpClient;

        public Client(ClientConfiguration configuration)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = configuration.BaseUri
            };
            if (configuration.AuthenticationHeaderValue != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = configuration.AuthenticationHeaderValue;
            }
            
            _httpClient = httpClient;
        }

        public void UpdateAuthentication(AuthenticationHeaderValue authenticationHeaderValue) => _httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

        public async Task<OneOf<GetUser, NotFound, Error>> GetUserByEmail(string email, CancellationToken token = default)
        {
            using var response = await _httpClient.GetAsync("user", email, token: token).ConfigureAwait(false);

            return await response.Content.HandleResultOrNotFoundOrError<GetUser>(response.StatusCode, HttpStatusCode.OK).ConfigureAwait(false);
        }

        public async Task<OneOf<int, Error>> AddUser(AddUser user, CancellationToken token = default)
        {
            using var inputContent = user.GetStringInputContent();
            using var response = await _httpClient.PostAsync("user", content: inputContent, token: token).ConfigureAwait(false);

            return await response.Content.HandleResultOrError<int>(response.StatusCode, HttpStatusCode.Created).ConfigureAwait(false);
        }

        public async Task<OneOf<int, Error>> AddUserRaw(byte[] user, CancellationToken token = default)
        {
            using var inputContent = user.GetByteArrayContentWithApplicationJsonContentType();
            using var response = await _httpClient.PostAsync("user", content: inputContent, token: token).ConfigureAwait(false);

            return await response.Content.HandleResultOrError<int>(response.StatusCode, HttpStatusCode.Created).ConfigureAwait(false);
        }

        public void Dispose() => _httpClient.Dispose();
    }
}