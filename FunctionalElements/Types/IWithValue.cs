namespace FunctionalElements.Types;

public interface IWithValue<out T>
{
    T Value { get; }
}