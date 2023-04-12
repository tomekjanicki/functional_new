using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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

        public void UpdateAuthentication(AuthenticationHeaderValue authenticationHeaderValue)
        {
            _httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
        }

        public async Task<OneOf<GetUser, NotFound, Error>> GetUserByEmail(string email, CancellationToken token = default)
        {
            var response = await _httpClient.GetAsync("user", email, token: token).ConfigureAwait(false);

            return await HandleResultOrNotFoundOrError<GetUser>(response.Content, response.StatusCode, HttpStatusCode.OK).ConfigureAwait(false);
        }

        public async Task<OneOf<int, Error>> AddUser(AddUser user, CancellationToken token = default)
        {
            var response = await _httpClient.PostAsync("user", content: new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json"), token: token).ConfigureAwait(false);

            return await HandleResultOrError<int>(response.Content, response.StatusCode, HttpStatusCode.Created).ConfigureAwait(false);
        }

        public async Task<OneOf<int, Error>> AddUserRaw(byte[] user, CancellationToken token = default)
        {
            var response = await _httpClient.PostAsync("user", content: new ByteArrayContent(user), token: token).ConfigureAwait(false);

            return await HandleResultOrError<int>(response.Content, response.StatusCode, HttpStatusCode.Created).ConfigureAwait(false);
        }

        private static async Task<OneOf<T, Error>> HandleResultOrError<T>(HttpContent content, HttpStatusCode statusCode,
            HttpStatusCode successStatusCode)
        {
            var contentAsString = await content.ReadAsStringAsync().ConfigureAwait(false);
            return statusCode == successStatusCode
                ? (OneOf<T, Error>)Deserialize<T>(contentAsString)
                : new Error(contentAsString, statusCode);
        }

        private static async Task<OneOf<T, NotFound, Error>> HandleResultOrNotFoundOrError<T>(HttpContent content, HttpStatusCode statusCode,
            HttpStatusCode successStatusCode)
        {
            var contentAsString = await content.ReadAsStringAsync().ConfigureAwait(false);
            if (statusCode == successStatusCode)
            {
                return Deserialize<T>(contentAsString);
            }
            if (statusCode == HttpStatusCode.NotFound)
            {
                return new NotFound();
            }
            return new Error(contentAsString, statusCode);
        }

        private static T Deserialize<T>(string contentAsString)
        {
            return JsonSerializer.Deserialize<T>(contentAsString)!;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}