using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MS.Katusha.Infrastructure.Cache
{
    public class KatushaGlobalMemoryCacheContext : IKatushaGlobalCacheContext
    {
        private static IDictionary<string, CacheObject> _dictionary = new Dictionary<string,CacheObject>();
        private static readonly ReaderWriterLockSlim ListLock = new ReaderWriterLockSlim();

        public void Add<T>(string key, T value) where T : class
        {
            var containsKey = ContainsKey(key);
            if (!containsKey) {
                ListLock.EnterWriteLock();
                try {
                    _dictionary[key] = new CacheObject {Key = key, Value = value};
                } finally {
                    ListLock.ExitWriteLock();
                }
            }
        }

        private static bool ContainsKey(string key)
        {
            bool containsKey;
            ListLock.EnterReadLock();
            try {
                containsKey = _dictionary.ContainsKey(key);
            } finally {
                ListLock.ExitReadLock();
            }
            return containsKey;
        }

        public T Get<T>(string key) where T : class
        {
            if (ContainsKey(key)) {
                ListLock.EnterReadLock();
                try {
                    return (T) _dictionary[key].Value;
                }finally {
                    ListLock.ExitReadLock();
                }
            }
            return default(T);
        }

        public void Delete(string key)
        {
            ListLock.EnterWriteLock();
            try {
                _dictionary.Remove(key);
            } finally {
                ListLock.ExitWriteLock();
            }
        }

        public void Clear(string prefix = "")
        {
            ListLock.EnterWriteLock();
            try {
                if (string.IsNullOrWhiteSpace(prefix)) {
                    _dictionary = new Dictionary<string, CacheObject>();

                } else {
                    foreach (var item in _dictionary.Where(item => item.Key.StartsWith(prefix))) {
                        _dictionary.Remove(item.Key);
                    }
                }
            } finally {
                ListLock.ExitWriteLock();
            }
        }
    }
}