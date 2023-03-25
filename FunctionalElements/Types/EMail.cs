using System.Text.RegularExpressions;
using OneOf;
using OneOf.Types;

namespace FunctionalElements.Types;

public sealed record EMail
{
    private static readonly Regex Regex = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

    private EMail(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static explicit operator EMail(string value)
    {
        return Extensions.GetValueWhenSuccessOrThrowInvalidCastException(() => TryCreate(value));
    }

    public static implicit operator string(EMail value)
    {
        return value.Value;
    }

    public static OneOf<EMail, Error<string>> TryCreate(string? value)
    {
        if (value is null)
        {
            return new Error<string>("value is null");
        }
        var match = Regex.Match(value);

        return match.Success ? new EMail(value) : new Error<string>("value is not valid email");
    }
}

public static class Extensions
{
    public static T GetValueWhenSuccessOrThrowInvalidCastException<T>(Func<OneOf<T, Error<string>>> tryCreateFunc)
    {
        var result = tryCreateFunc();

        if (result.IsT0)
        {
            return result.AsT0;
        }

        throw new InvalidCastException(result.AsT1.Value);
    }
}