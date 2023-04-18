using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ApiClient.V2.Models.Abstractions;

namespace ApiClient
{
    public abstract class BaseWithCustomFragmentsJsonConverter<T> : JsonConverter<T>
        where T : class, IWithCustomFragments
    {
        private readonly Func<T> _constructorFunc;
        private readonly IReadOnlyDictionary<string, Type> _additionalPropertyClasses;

        protected BaseWithCustomFragmentsJsonConverter(Func<T> constructorFunc, IReadOnlyDictionary<string, Type> additionalPropertyClasses)
        {
            _constructorFunc = constructorFunc;
            _additionalPropertyClasses = additionalPropertyClasses;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var instance = _constructorFunc();
            var additionalObjects = new Dictionary<string, object?>();
            var instanceProperties = typeToConvert.GetTypeInfo().DeclaredProperties.ToList();
            using (var jsonDocument = JsonDocument.ParseValue(ref reader))
            {
                var objectEnumerator = jsonDocument.RootElement.EnumerateObject();
                while (objectEnumerator.MoveNext())
                {
                    var current = objectEnumerator.Current;
                    var property = FindProperty(instanceProperties, current);
                    if (property != null)
                    {
                        property.SetValue(instance, current.Value.Deserialize(property.PropertyType, options));

                        continue;
                    }

                    HandleFillingCustomFragments(options, current, additionalObjects);
                }
            }
            instance.CustomFragments = additionalObjects;

            return instance;
        }

        private void HandleFillingCustomFragments(JsonSerializerOptions options, JsonProperty current, IDictionary<string, object?> additionalObjects)
        {
            additionalObjects.Add(current.Name,
                _additionalPropertyClasses.TryGetValue(current.Name, out var type)
                    ? current.Value.Deserialize(type, options)
                    : current.Value.Deserialize<object>(options));
        }

        private static PropertyInfo? FindProperty(List<PropertyInfo> instanceProperties, JsonProperty current)
        {
            return instanceProperties.Find(propertyInfo =>
            {
                var attribute = GetJsonPropertyNameAttribute(propertyInfo);

                return current.NameEquals(attribute == null ? propertyInfo.Name : attribute.Name);
            });
        }

        private static JsonPropertyNameAttribute? GetJsonPropertyNameAttribute(MemberInfo propertyInfo)
        {
            var attribute = Attribute.GetCustomAttribute(propertyInfo, typeof(JsonPropertyNameAttribute));

            return (JsonPropertyNameAttribute)attribute;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            var type = value.GetType();
            foreach (var property in type.GetProperties())
            {
                if (!property.CanRead)
                {
                    continue;
                }
                var propertyValue = property.GetValue(value, null);
                if (propertyValue == null)
                {
                    continue;
                }
                if (typeof(IReadOnlyDictionary<string, object?>).IsAssignableFrom(property.PropertyType))
                {
                    HandleDictionarySerialization(writer, options, propertyValue);

                    continue;
                }

                HandleRegularPropertySerialization(writer, options, property, propertyValue);
            }
            writer.WriteEndObject();
        }

        private static void HandleRegularPropertySerialization(Utf8JsonWriter writer, JsonSerializerOptions options, MemberInfo property, object propertyValue)
        {
            var attribute = GetJsonPropertyNameAttribute(property);
            if (attribute != null)
            {
                var jsonPropertyName = attribute.Name;
                writer.WritePropertyName(jsonPropertyName);
                JsonSerializer.Serialize(writer, propertyValue, options);

                return;
            }
            writer.WritePropertyName(property.Name);
            JsonSerializer.Serialize(writer, propertyValue, options);
        }

        private static void HandleDictionarySerialization(Utf8JsonWriter writer, JsonSerializerOptions options, object propertyValue)
        {
            var dictionary = (IReadOnlyDictionary<string, object?>)propertyValue;
            foreach (var item in dictionary)
            {
                writer.WritePropertyName(item.Key);
                JsonSerializer.Serialize(writer, item.Value, options);
            }
        }
    }
}