using System;
using System.Configuration;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using ServiceStack.Redis;

namespace MS.Katusha.Redis
{
    public class KatushaGlobalRedisCacheContext : IKatushaGlobalCacheContext
    {
        private readonly Uri _connectionUri;
        private readonly PooledRedisClientManager _redisClientManager;

        public KatushaGlobalRedisCacheContext(string redisUrlName = "REDISTOGO_URL")
        {
            var redisUrl = ConfigurationManager.AppSettings.Get(redisUrlName);
            _connectionUri = new Uri("ubuntu.katusha.com:6379");
            _redisClientManager = new PooledRedisClientManager(_connectionUri.ToString());
        }

        public void Add<T>(string key, T value)
        {
            using (var redis = _redisClientManager.GetCacheClient()) {
                redis.Add(key, value);
                //value = redis.Get<string>("hello");
            }
        }

        public T Get<T>(string key)
        {
            using (var redis = _redisClientManager.GetCacheClient()) {
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