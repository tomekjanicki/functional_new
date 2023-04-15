using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiClient
{
    public sealed class ReadOnlyCollectionJsonConvertFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var types = typeToConvert.GetGenericArguments();
            var type = typeof(ReadOnlyCollectionJsonConverter<>).MakeGenericType(types[0]);

            return (JsonConverter)Activator.CreateInstance(
                type,
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: Array.Empty<object>(),
                culture: null)!;
        }

        private sealed class ReadOnlyCollectionJsonConverter<T> : JsonConverter<IReadOnlyCollection<T>>
        {
            public override bool HandleNull => true;

            public override IReadOnlyCollection<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return new List<T>();
                }
                var valueType = typeof(IEnumerable<T>);
                var valueConverter = (JsonConverter<IEnumerable<T>>)options.GetConverter(valueType);

                return new List<T>(valueConverter.Read(ref reader, valueType, options)!);
            }

            public override void Write(Utf8JsonWriter writer, IReadOnlyCollection<T> value, JsonSerializerOptions options)
            {
                var valueType = typeof(IEnumerable<T>);
                var valueConverter = (JsonConverter<IEnumerable<T>?>)options.GetConverter(valueType);
                valueConverter.Write(writer, value, options);
            }
        }
    }
}