using System.Net;

namespace ApiClient.Infrastructure
{
    public sealed class Error
    {
        public Error(string content, HttpStatusCode statusCode)
        {
            Content = content;
            StatusCode = statusCode;
        }

        public string Content { get; }

        public HttpStatusCode StatusCode { get; }
    }
}