using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace MS.Katusha.Infrastructure.Cache
{
    public class KatushaGlobalMemoryCacheContext : IKatushaGlobalCacheContext
    {
        private static readonly IDictionary<string, CacheObject> Dictionary = new Dictionary<string,CacheObject>();
        private static readonly ReaderWriterLockSlim ListLock = new ReaderWriterLockSlim();

        public void Add<T>(string key, T value)
        {
            var containsKey = ContainsKey(key);
            if (containsKey) {
                ListLock.EnterWriteLock();
                try {
                    Dictionary[key] = new CacheObject {Key = key, Value = value};
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
                containsKey = Dictionary.ContainsKey(key);
            } finally {
                ListLock.ExitReadLock();
            }
            return containsKey;
        }

        public T Get<T>(string key)
        {
            if (ContainsKey(key)) {
                ListLock.EnterReadLock();
                try {
                    return (T) Dictionary[key].Value;
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
                Dictionary.Remove(key);
            } finally {
                ListLock.ExitWriteLock();
            }
        }

        public bool Contains(string key) { return ContainsKey(key); }
        
        public int Count(Expression<Func<CacheObject, bool>> expression)
        {
            ListLock.EnterReadLock();
            try {
                return Dictionary.Count;
            } finally {
                ListLock.ExitWriteLock();
            }
        }

        public IList<CacheObject> Get(Expression<Func<CacheObject, bool>> expression) { throw new NotImplementedException(); }
    }
}