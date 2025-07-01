using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CSUtils
{
    /// <summary>
    /// A 2-way dictionary.
    /// </summary>
    /// <typeparam name="T">The type of key1.</typeparam>
    /// <typeparam name="U">The type of key2.</typeparam>
    public class KeyMap<T, U> : IEnumerable<(T, U)>
        where T : notnull
        where U : notnull
    {
        private readonly Dictionary<T, U> _tDict;
        private readonly Dictionary<U, T> _uDict;

        public KeyMap()
            : this(0)
        {
        }

        public KeyMap(int capacity = 0)
        {
            _tDict = new(capacity);
            _uDict = new(capacity);
        }

        public KeyMap(IEnumerable<(T, U)> collection)
            : this()
        {
            AddRange(collection);
        }

        public int Count { get => _tDict.Count; }

        #region Add

        public void Add(T t, U u)
        {
            _tDict.Add(t, u);
            _uDict.Add(u, t);
        }

        public void Add((T, U) pair)
        {
            Add(pair.Item1, pair.Item2);
        }

        public void AddRange(IEnumerable<(T, U)> collection)
        {
            foreach (var pair in collection)
            {
                Add(pair);
            }
        }

        public void AddEvery(params (T, U)[] values)
        {
            AddRange(values);
        }

        #endregion

        #region Remove

        public bool Remove(T key1, U key2)
        {
            if (!(_tDict.ContainsKey(key1) && _uDict.ContainsKey(key2)))
            {
                return false;
            }

            _tDict.Remove(key1);
            _uDict.Remove(key2);

            return true;
        }

        public bool RemoveKeyT(T key1)
        {
            if (!_tDict.ContainsKey(key1))
            {
                return false;
            }

            return Remove(key1, _tDict[key1]);
        }

        public bool RemoveKeyU(U key2)
        {
            if (!_uDict.ContainsKey(key2))
            {
                return false;
            }

            return Remove(_uDict[key2], key2);
        }

        #endregion

        #region ContainsKey

        public bool ContainsKeyT(T key1)
        {
            return _tDict.ContainsKey(key1);
        }

        public bool ContainsKeyU(U key2)
        {
            return _uDict.ContainsKey(key2);
        }

        #endregion

        #region Get

        public U GetU(T key1)
        {
            return _tDict[key1];
        }

        public T GetT(U key2)
        {
            return _uDict[key2];
        }

        public bool TryGetU(T key1, [NotNullWhen(true)] out U? value)
        {
            return _tDict.TryGetValue(key1, out value);
        }

        public bool TryGetT(U key2, [NotNullWhen(true)] out T? value)
        {
            return _uDict.TryGetValue(key2, out value);
        }

        #endregion

        #region Clear

        public void Clear()
        {
            _tDict.Clear();
            _uDict.Clear();
        }

        #endregion

        #region IEnumerable

        public IEnumerator<(T, U)> GetEnumerator()
        {
            foreach (var pair in _tDict)
            {
                yield return (pair.Key, pair.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}