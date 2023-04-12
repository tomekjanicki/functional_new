using System.Text.Json.Serialization;

namespace ApiClient.Models.Dtos
{
    public sealed class GetUser
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("eMail")]
        public string EMail { get; set; } = string.Empty;

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
    }
}