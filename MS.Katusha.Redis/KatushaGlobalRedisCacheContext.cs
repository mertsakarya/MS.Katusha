using MS.Katusha.Infrastructure.Cache;
using ServiceStack.Redis;

namespace MS.Katusha.Redis
{
    public class KatushaGlobalRedisCacheContext : KatushaGlobalBaseCacheContext
    {
        private readonly IRedisClientsManager _redisClientManager;

        public KatushaGlobalRedisCacheContext(IRedisClientsManager redisClientManager, IKatushaGlobalCacheContext baseCacheContext = null)
            : base(baseCacheContext)
        {
            _redisClientManager = redisClientManager; 
        }

        public override void Add<T>(string key, T value)
        {
            base.Add(key, value);
            if (value == null)
                Delete(key);
            else
                using (var redis = _redisClientManager.GetCacheClient()) {
                    if (Get<T>(key) != null)
                        redis.Remove(key);
                    redis.Add(key, value);
                }
        }

        public override T Get<T>(string key)
        {
            var value = base.Get<T>(key);
            if (value != null) return value;
            using (var redis = _redisClientManager.GetCacheClient()) {
                try {
                    return redis.Get<T>(key);
                } catch {
                    return default(T);
                }
            }
        }

        public override void Delete(string key)
        {
            base.Delete(key);
            using (var redis = _redisClientManager.GetCacheClient()) {
                redis.Remove(key);
            }
        }
    }
}