using System;
using System.Net.Http.Headers;

namespace ApiClient.V2.Abstractions
{
    public interface IRootClient : IDisposable
    {
        void UpdateAuthentication(AuthenticationHeaderValue authenticationHeaderValue);

        IEventApi EventApi { get; }

        IManagedObjectApi ManagedObjectApi { get; }
    }
}