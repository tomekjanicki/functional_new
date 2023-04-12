using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ApiClient.Models.Dtos;
using OneOf;
using OneOf.Types;
using Error = ApiClient.Infrastructure.Error;

namespace ApiClient.Services.Abstractions
{
    public interface IClient : IDisposable
    {
        void UpdateAuthentication(AuthenticationHeaderValue authenticationHeaderValue);

        Task<OneOf<GetUser, NotFound, Error>> GetUserByEmail(string email, CancellationToken token = default);

        Task<OneOf<int, Error>> AddUser(AddUser user, CancellationToken token = default);

        Task<OneOf<int, Error>> AddUserRaw(byte[] user, CancellationToken token = default);
    }
}