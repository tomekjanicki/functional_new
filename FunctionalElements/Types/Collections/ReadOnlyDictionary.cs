using System.Collections;

namespace FunctionalElements.Types.Collections;

public sealed class ReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dictionary;

    public ReadOnlyDictionary()
        : this(new Dictionary<TKey, TValue>())
    {
    }

    public ReadOnlyDictionary(Dictionary<TKey, TValue> dictionary) => _dictionary = dictionary;

    public Enumerator GetEnumerator() => new(this);

    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<TKey, TValue>.Enumerator _enumerator;

        public Enumerator(ReadOnlyDictionary<TKey, TValue> set) => _enumerator = set._dictionary.GetEnumerator();

        public bool MoveNext() => _enumerator.MoveNext();

        public KeyValuePair<TKey, TValue> Current => _enumerator.Current;

        public void Dispose()
        {
        }

        object IEnumerator.Current => _enumerator.Current;

        void IEnumerator.Reset() => ((IEnumerator)_enumerator).Reset();
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _dictionary.Count;

    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

    public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value!);

    public TValue this[TKey key] => _dictionary[key];

    public TValue GetValueOrDefault(TKey key, TValue defaultValue) => _dictionary.TryGetValue(key, out var value) ? value : defaultValue;
}