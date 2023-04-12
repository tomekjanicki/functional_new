using System;
using System.Net.Http.Headers;

namespace ApiClient.Models
{
    public sealed class ClientConfiguration
    {
        public ClientConfiguration(Uri baseUri, AuthenticationHeaderValue? authenticationHeaderValue = null)
        {
            BaseUri = baseUri;
            AuthenticationHeaderValue = authenticationHeaderValue;
        }

        public Uri BaseUri { get; }

        public AuthenticationHeaderValue? AuthenticationHeaderValue { get; } 
    }
}