using MS.Katusha.Interfaces.Repositories;

namespace MS.Katusha.Infrastructure.Cache
{
    public class KatushaGlobalRavenCacheContext : KatushaGlobalBaseCacheContext
    {
        private readonly IRepository<CacheObject> _repository;

        public KatushaGlobalRavenCacheContext(IRepository<CacheObject> repository, IKatushaGlobalCacheContext baseCacheContext = null) : base(baseCacheContext)  {
            _repository = repository;
        }

        public override void Add<T>(string key, T value)
        {
            base.Add(key, value);
            var existingObject = _repository.SingleAttached(p => p.Key == key);
            if (existingObject != null) {
                existingObject.Value = value;
                _repository.FullUpdate(existingObject);
            } else {
                var cacheObject = new CacheObject { Key = key, Value = value };
                _repository.Add(cacheObject);
            }
        }

        public override T Get<T>(string key)
        {
            var value = base.Get<T>(key);
            if (value != null) return value;
            var existingObject = _repository.Single(p => p.Key == key);
            if (existingObject != null)
                return (T)existingObject.Value;
            return default(T);
        }

        public override void Delete(string key)
        {
            base.Delete(key);
            var existingObject = _repository.SingleAttached(p => p.Key == key);
            if (existingObject != null) {
                _repository.Delete(existingObject);
            }
        }
    }
}