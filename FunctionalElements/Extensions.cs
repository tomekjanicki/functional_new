using FunctionalElements.Types.Collections;
using Microsoft.OpenApi.Models;
using OneOf;
using OneOf.Types;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FunctionalElements;

public static class Extensions
{
    public static T GetValueWhenSuccessOrThrowInvalidCastException<T>(Func<OneOf<T, Error<string>>> tryCreateFunc)
    {
        return tryCreateFunc().Match(arg => arg, error => throw new InvalidCastException(error.Value));
    }

    public static ReadOnlyDictionary<string, string> ToReadOnlyDictionary(this string value)
    {
        return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { string.Empty, value  }
        });
    }

    public static ReadOnlyDictionary<string, string> ToReadOnlyDictionary(this Error<string> error)
    {
        return error.Value.ToReadOnlyDictionary();
    }

    public static void MapTypeToString<T>(this SwaggerGenOptions swaggerGenOptions)
    {
        swaggerGenOptions.MapType<T>(() => new OpenApiSchema
        {
            Type = "string"
        });
    }
}