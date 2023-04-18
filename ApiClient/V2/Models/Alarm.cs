using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ApiClient.V2.Models.Abstractions;

namespace ApiClient.V2.Models
{
    [JsonConverter(typeof(JsonConverterAwareFragments))]
    public sealed class Alarm : IWithCustomFragments
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("customFragments")]
        public IReadOnlyDictionary<string, object?> CustomFragments { get; set; } = new Dictionary<string, object?>();

        private static readonly IDictionary<string, Type> AdditionalPropertyClasses = new Dictionary<string, Type>();

        public static bool TryAddProperty(string key, Type type)
        {
            return AdditionalPropertyClasses.TryAdd(key, type);
        }

        public static bool RemoveProperty(string key)
        {
            return AdditionalPropertyClasses.Remove(key);
        }

        private sealed class JsonConverterAwareFragments : BaseWithCustomFragmentsJsonConverter<Alarm>
        {
            public JsonConverterAwareFragments()
                : base(() => new Alarm(), new Dictionary<string, Type>(AdditionalPropertyClasses))
            {
            }
        }
    }
}