using System.Collections;

namespace CSUtils
{
    /// <summary>
    /// Represents an item and it's quantity.
    /// </summary>
    public readonly record struct QuantityPair<T> : IComparable<QuantityPair<T>>
        where T : notnull
    {
        public T Item { get; init; }

        public int Quantity { get; init; }

        public static explicit operator QuantityPair<T>(KeyValuePair<T, int> pair)
        {
            return new()
            {
                Item = pair.Key,
                Quantity = pair.Value,
            };
        }

        public void Deconstruct(out T item, out int quantity)
        {
            item = Item;
            quantity = Quantity;
        }

        public int CompareTo(QuantityPair<T> other)
        {
            return Quantity - other.Quantity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item, Quantity);
        }

        public override string ToString()
        {
            return string.Join("{ ", Item, ", ", Quantity, " }");
        }
    }

    /// <summary>
    /// Represents a set of unique values and quantities.
    /// </summary>
    public class QuantitySet<T> : IEnumerable<QuantityPair<T>>
        where T : notnull
    {
        private readonly Dictionary<T, int> _dict;

        public QuantitySet()
        {
            _dict = new();
        }

        public QuantitySet(int capacity)
        {
            _dict = new(capacity);
        }

        public QuantitySet(IEnumerable<QuantityPair<T>> collection)
            : this()
        {
            AddRange(collection);
        }

        public int UniqueCount { get => _dict.Count; }

        public int TotalCount()
        {
            return _dict.Sum(pair => pair.Value);
        }

        public IEnumerable<T> Items { get => _dict.Keys; }

        public IEnumerable<int> Quantities { get => _dict.Values; }

        #region Modify

        public int this[T item]
        {
            get => _dict[item];
            set => _dict[item] = value;
        }

        public bool Add(T item, int quantity = 1)
        {
            if (quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative.");
            }

            if (quantity == 0)
            {
                return false;
            }

            if (_dict.TryGetValue(item, out var curQuantity))
            {
                _dict[item] = curQuantity + quantity;
            }
            else
            {
                _dict[item] = quantity;
            }

            return true;
        }

        public void Add(QuantityPair<T> pair)
        {
            Add(pair.Item, pair.Quantity);
        }

        public void AddRange(IEnumerable<QuantityPair<T>> collection)
        {
            foreach (var pair in collection)
            {
                Add(pair);
            }
        }

        public int Remove(T item, int quantity = 1)
        {
            if (!_dict.TryGetValue(item, out var curQuantity))
            {
                return 0;
            }

            int remove = Math.Min(quantity, curQuantity);

            int next = curQuantity - remove;

            _dict[item] = next;

            if (next == 0)
            {
                _dict.Remove(item);
            }

            return remove;
        }

        public int Remove(QuantityPair<T> pair)
        {
            return Remove(pair.Item, pair.Quantity);
        }

        public int RemoveRange(IEnumerable<QuantityPair<T>> collection)
        {
            int removed = 0;

            foreach (var pair in collection)
            {
                removed += Remove(pair);
            }

            return removed;
        }

        public void Set(T item, int quantity)
        {
            _dict[item] = quantity;
        }

        public bool TrySet(T value, int quantity)
        {
            if (!_dict.ContainsKey(value))
            {
                return false;
            }

            _dict[value] = quantity;

            return true;
        }

        public void Clear()
        {
            _dict.Clear();
        }

        #endregion

        #region Contains

        public bool Contains(T value)
        {
            return _dict.ContainsKey(value);
        }

        public int Quantity(T value)
        {
            if (_dict.TryGetValue(value, out var quantity))
            {
                return quantity;
            }

            return 0;
        }

        #endregion

        #region IEnumerable

        public IEnumerator<QuantityPair<T>> GetEnumerator()
        {
            foreach (var pair in _dict)
            {
                yield return (QuantityPair<T>)pair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
