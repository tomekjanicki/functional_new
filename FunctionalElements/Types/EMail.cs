using System.Text.RegularExpressions;
using FunctionalElements.Types.Abstractions;
using OneOf;
using OneOf.Types;

namespace FunctionalElements.Types;

public sealed record EMail : IWithValue<string>
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
        if (value.Length > 320)
        {
            return new Error<string>("value is greater than 320 chars");
        }
        var match = Regex.Match(value);

        return match.Success ? new EMail(value) : new Error<string>("value is not valid email");
    }
}