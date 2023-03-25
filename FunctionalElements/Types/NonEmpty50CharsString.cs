using OneOf.Types;
using OneOf;

namespace FunctionalElements.Types;

public sealed record NonEmpty50CharsString
{
    private NonEmpty50CharsString(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static explicit operator NonEmpty50CharsString(string value)
    {
        return Extensions.GetValueWhenSuccessOrThrowInvalidCastException(() => TryCreate(value));
    }

    public static implicit operator string(NonEmpty50CharsString value)
    {
        return value.Value;
    }

    public static OneOf<NonEmpty50CharsString, Error<string>> TryCreate(string? value)
    {
        return value switch
        {
            null => new Error<string>("value is null"),
            "" => new Error<string>("value is empty"),
            _ => value.Length > 50 ? new Error<string>("value is greater than 50 chars") : new NonEmpty50CharsString(value)
        };
    }
}