using System.Text.Json.Serialization;

namespace Consumer;

public sealed class UserDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("eMail")] 
    public string EMail { get; init; } = string.Empty;

    [JsonPropertyName("firstName")] 
    public string FirstName { get; init; } = string.Empty;

    [JsonPropertyName("lastName")] 
    public string LastName { get; init; } = string.Empty;
}