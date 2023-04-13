using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
        private const string ApplicationJsonContentType = "application/json";

        public static Task<HttpResponseMessage> PostAsync(this HttpClient httpClient, string path, string? pathParam = null,
            IReadOnlyDictionary<string, object?>? queryParams = null, HttpContent? content = null, IEnumerable<Header>? headers = null, CancellationToken token = default) =>
            httpClient.SendAsync(HttpMethod.Post, path, pathParam, queryParams, content, headers, token);

        public static Task<HttpResponseMessage> GetAsync(this HttpClient httpClient, string path, string? pathParam = null,
            IReadOnlyDictionary<string, object?>? queryParams = null, HttpContent? content = null, IEnumerable<Header>? headers = null, CancellationToken token = default) =>
            httpClient.SendAsync(HttpMethod.Get, path, pathParam, queryParams, content, headers, token);

        public static async Task<HttpResponseMessage> SendAsync(this HttpClient httpClient, HttpMethod method, string path, string? pathParam = null, IReadOnlyDictionary<string, object?>? queryParams = null,
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
            using var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = uriBuilder.Uri,
                Content = content
            };
            if (headers == null)
            {
                return await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false);
            }
            foreach (var header in headers)
            {
                request.Headers.Add(header.Name, header.Values);
            }

            return await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false);
        }

        public static string GetUrlParameters(this IReadOnlyDictionary<string, object?> parameters)
        {
            var queryParamCollection = HttpUtility.ParseQueryString(string.Empty);
            foreach (var parameter in parameters)
            {
                if (parameter.Value != null)
                {
                    queryParamCollection.Add(parameter.Key, GetStringValue(parameter.Value));
                }
            }
            return queryParamCollection.ToString();
        }

        public static string GetStringValue(object input)
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

        private static void ThrowErrorException(Error error) => throw new InvalidOperationException($"Api method failed with Status Code: {error.StatusCode}, Content: {error.Content}");

        public static StringContent GetStringInputContent<T>(this T input) => new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, ApplicationJsonContentType);

        public static ByteArrayContent GetByteArrayContentWithApplicationJsonContentType(this byte[] array)
        {
            var content = new ByteArrayContent(array);
            content.Headers.ContentType = new MediaTypeHeaderValue(ApplicationJsonContentType);

            return content;
        }

        public static T Deserialize<T>(this string contentAsString) => JsonSerializer.Deserialize<T>(contentAsString)!;

        public static async Task<OneOf<T, Error>> HandleResultOrError<T>(this HttpContent content, HttpStatusCode statusCode,
            HttpStatusCode successStatusCode)
        {
            var contentAsString = await content.ReadAsStringAsync().ConfigureAwait(false);
            return statusCode == successStatusCode
                ? (OneOf<T, Error>)contentAsString.Deserialize<T>()
                : new Error(contentAsString, statusCode);
        }

        public static async Task<OneOf<T, NotFound, Error>> HandleResultOrNotFoundOrError<T>(this HttpContent content, HttpStatusCode statusCode,
            HttpStatusCode successStatusCode)
        {
            var contentAsString = await content.ReadAsStringAsync().ConfigureAwait(false);
            if (statusCode == successStatusCode)
            {
                return contentAsString.Deserialize<T>();
            }
            if (statusCode == HttpStatusCode.NotFound)
            {
                return new NotFound();
            }
            return new Error(contentAsString, statusCode);
        }
    }
}