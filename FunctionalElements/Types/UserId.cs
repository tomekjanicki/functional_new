using FunctionalElements.Types.Abstractions;

namespace FunctionalElements.Types;

public readonly record struct UserId(int Value) : IWithValue<int>
{
    public static explicit operator UserId(int value) => new(value);

    public static implicit operator int(UserId value) => value.Value;
}