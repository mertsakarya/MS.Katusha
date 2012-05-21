using MS.Katusha.Infrastructure.Cache;
using ServiceStack.Redis;

namespace MS.Katusha.Redis
{
    public class KatushaGlobalRedisCacheContext : IKatushaGlobalCacheContext
    {
        private readonly IRedisClientsManager _redisClientManager;

        public KatushaGlobalRedisCacheContext(IRedisClientsManager redisClientManager)
        {
            _redisClientManager = redisClientManager; 
        }

        public void Add<T>(string key, T value)
        {
            using (var redis = _redisClientManager.GetCacheClient()) {
                redis.Add(key, value);
            }
        }

        public T Get<T>(string key)
        {
            using (var redis = _redisClientManager.GetReadOnlyCacheClient()) {
                return redis.Get<T>(key);
            }
        }

        public void Delete(string key)
        {
            using (var redis = _redisClientManager.GetCacheClient()) {
                redis.Remove(key);
            }
        }
    }
}