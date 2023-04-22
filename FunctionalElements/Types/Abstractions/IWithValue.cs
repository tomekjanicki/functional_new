namespace FunctionalElements.Types.Abstractions;

public interface IWithValue<out T>
{
    T Value { get; }
}