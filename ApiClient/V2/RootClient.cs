using System;
using System.Net.Http;
using System.Net.Http.Headers;
using ApiClient.Models;
using ApiClient.V2.Abstractions;

namespace ApiClient.V2
{
    public sealed class RootClient : IRootClient
    {
        private readonly HttpClient _httpClient;
        private readonly Lazy<IEventApi> _lazyEventApi;
        private readonly Lazy<IManagedObjectApi> _lazyManagedObjectApi;

        public RootClient(ClientConfiguration configuration)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = configuration.BaseUri
            };
            if (configuration.AuthenticationHeaderValue != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = configuration.AuthenticationHeaderValue;
            }

            _lazyEventApi = new Lazy<IEventApi>(() => new EventApi(httpClient));
            _lazyManagedObjectApi = new Lazy<IManagedObjectApi>(() => new ManagedObjectApi(httpClient));
            _httpClient = httpClient;
        }

        public void UpdateAuthentication(AuthenticationHeaderValue authenticationHeaderValue) => _httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

        public IEventApi EventApi => _lazyEventApi.Value;
        public IManagedObjectApi ManagedObjectApi => _lazyManagedObjectApi.Value;
        public void Dispose() => _httpClient.Dispose();
    }
}