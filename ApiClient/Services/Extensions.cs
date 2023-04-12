using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ApiClient.Infrastructure;
using ApiClient.Models;
using ApiClient.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using Error = ApiClient.Infrastructure.Error;

namespace ApiClient.Services
{
    public static class Extensions
    {
        public static Task<HttpResponseMessage> PostAsync(this HttpClient httpClient, string path, string? pathParam = null,
            IReadOnlyDictionary<string, object?>? queryParams = null, HttpContent? content = null, IEnumerable<Header>? headers = null, CancellationToken token = default)
        {
            return httpClient.SendAsync(HttpMethod.Post, path, pathParam, queryParams, content, headers, token);
        }

        public static Task<HttpResponseMessage> GetAsync(this HttpClient httpClient, string path, string? pathParam = null,
            IReadOnlyDictionary<string, object?>? queryParams = null, HttpContent? content = null, IEnumerable<Header>? headers = null, CancellationToken token = default)
        {
            return httpClient.SendAsync(HttpMethod.Get, path, pathParam, queryParams, content, headers, token);
        }

        public static Task<HttpResponseMessage> SendAsync(this HttpClient httpClient, HttpMethod method, string path, string? pathParam = null, IReadOnlyDictionary<string, object?>? queryParams = null,
            HttpContent? content = null, IEnumerable<Header>? headers = null, CancellationToken token = default)
        {
            var parameters = queryParams == null ? string.Empty : queryParams.GetUrlParameters();
            var pathBuilder = new StringBuilder();
            pathBuilder.Append(httpClient.BaseAddress);
            pathBuilder.Append(path);
            if (pathParam != null)
            {
                pathBuilder.Append('/');
                pathBuilder.Append(HttpUtility.UrlEncode(pathParam));
            }
            var uriBuilder = new UriBuilder(pathBuilder.ToString())
            {
                Query = parameters
            };
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = uriBuilder.Uri,
                Content = content,
            };
            if (headers == null)
            {
                return httpClient.SendAsync(request, token);
            }
            foreach (var header in headers)
            {
                request.Headers.Add(header.Name, header.Values);
            }

            return httpClient.SendAsync(request, token);
        }

        public static string GetUrlParameters(this IReadOnlyDictionary<string, object?> parameters)
        {
            var queryParamCollection = HttpUtility.ParseQueryString(string.Empty);
            foreach (var parameter in parameters)
            {
                if (parameter.Value != null)
                {
                    queryParamCollection.Add(parameter.Key, GetValue(parameter.Value));
                }
            }
            return queryParamCollection.ToString();
        }

        private static string GetValue(object input)
        {
            if (input is DateTime dateTime)
            {
                return dateTime.ToString("O");
            }

            return input.ToString() ?? string.Empty;
        }

        public static IServiceCollection AddClient(this IServiceCollection collection, Func<IServiceProvider, ClientConfiguration> clientConfigurationFactory)
        {
            collection.AddSingleton<IClient>(provider => new Client(clientConfigurationFactory(provider)));

            return collection;
        }

        public static T GetResultIfSuccessOrThrow<T>(this OneOf<T, NotFound, Error> result)
        {
            if (result.IsT0)
            {
                return result.AsT0;
            }
            if (result.IsT1)
            {
                throw new InvalidOperationException($"Api method failed with result: {result.AsT1}");
            }
            ThrowErrorException(result.AsT2);

            return default!;
        }

        public static T GetResultIfSuccessOrThrow<T>(this OneOf<T, Error> result)
        {
            if (result.IsT0)
            {
                return result.AsT0;
            }

            ThrowErrorException(result.AsT1);

            return default!;
        }

        private static void ThrowErrorException(Error error)
        {
            throw new InvalidOperationException($"Api method failed with Status Code: {error.StatusCode}, Content: {error.Content}");
        }
    }
}