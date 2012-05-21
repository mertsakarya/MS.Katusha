using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Interfaces.Repositories;

namespace MS.Katusha.Infrastructure.Cache
{
    public class KatushaGlobalRavenCacheContext : IKatushaGlobalCacheContext
    {
        private readonly IRepository<CacheObject> _repository;

        public KatushaGlobalRavenCacheContext(IRepository<CacheObject> repository) {
            _repository = repository;
        }

        public void Add<T>(string key, T value)
        {
            var existingObject = _repository.SingleAttached(p => p.Key == key);
            if (existingObject != null) {
                existingObject.Value = value;
                _repository.FullUpdate(existingObject);
            } else {
                var cacheObject = new CacheObject { Key = key, Value = value };
                _repository.Add(cacheObject);
            }
        }

        public T Get<T>(string key)
        {
            var existingObject = _repository.Single(p => p.Key == key);
            if (existingObject != null)
                return (T)existingObject.Value;
            return default(T);
        }

        public void Delete(string key)
        {
            var existingObject = _repository.SingleAttached(p => p.Key == key);
            if (existingObject != null) {
                _repository.Delete(existingObject);
            }
        }
    }
}