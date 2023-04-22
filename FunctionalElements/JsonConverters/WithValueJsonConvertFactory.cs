using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FunctionalElements.Types.Abstractions;

namespace FunctionalElements.JsonConverters;

public sealed class WithValueJsonConvertFactory : JsonConverterFactory
{
    private readonly Func<Type, bool> _typeSearchPredicate = type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IWithValue<>);

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.GetInterfaces().Any(_typeSearchPredicate);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var withValueType = typeToConvert.GetInterfaces().Single(_typeSearchPredicate);
        var types = withValueType.GetGenericArguments();
        var type = typeof(WithValueJsonConverter<>).MakeGenericType(types[0]);

        return (JsonConverter)Activator.CreateInstance(
            type,
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: Array.Empty<object>(),
            culture: null)!;
    }

    private sealed class WithValueJsonConverter<T> : JsonConverter<IWithValue<T>>
    {
        public override IWithValue<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, IWithValue<T> value, JsonSerializerOptions options)
        {
            var valueType = typeof(T);
            var valueConverter = (JsonConverter<T>)options.GetConverter(valueType);
            valueConverter.Write(writer, value.Value, options);
        }
    }
}