namespace FlameVM.Template
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class SafeDictionary<T, Q> : IDictionary<T, Q>
    {
        private readonly object guarder = new object();
        private readonly Dictionary<T, Q> _base = new Dictionary<T, Q>();
        private readonly Dictionary<Guid, T> _baseKeyHased = new Dictionary<Guid, T>();
        private readonly Dictionary<Guid, Q> _baseValueHased = new Dictionary<Guid, Q>();

        private void unWrapAdd(T t, Q q)
        {
            lock (guarder)
            {
                _base.Add(t, q);
                _baseKeyHased.Add(Guid.NewGuid(), t);
                _baseValueHased.Add(Guid.NewGuid(), q);
            }
        }

        public IEnumerator<KeyValuePair<T, Q>> GetEnumerator()
        {
            throw new NotSupportedException("Safe dictionary is not supported enumerator!");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<T, Q> item)
        {
            this.unWrapAdd(item.Key, item.Value);
        }

        public void Clear()
        {
            lock (guarder)
            {
                _base.Clear();
                _baseKeyHased.Clear();
                _baseValueHased.Clear();
            }
        }

        public bool Contains(KeyValuePair<T, Q> item)
        {
            lock (guarder)
            {
                return _base.ContainsKey(item.Key) && _base.ContainsValue(item.Value);
            }
        }

        public void CopyTo(KeyValuePair<T, Q>[] array, int arrayIndex)
        {
            throw new NotSupportedException("Safe dictionary is not supported enumerator!");
        }

        public bool Remove(KeyValuePair<T, Q> item)
        {
            throw new NotSupportedException("Safe dictionary is not supported enumerator!");
        }

        public int Count { get; }
        public bool IsReadOnly { get; }
        public bool ContainsKey(T key)
        {
            throw new System.NotImplementedException();
        }

        public void Add(T key, Q value)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(T key)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(T key, out Q value)
        {
            throw new System.NotImplementedException();
        }

        public Q this[T key]
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public ICollection<T> Keys { get; }
        public ICollection<Q> Values { get; }
    }
}