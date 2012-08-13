using System;

namespace MS.Katusha.Infrastructure.Cache
{
    public interface IKatushaGlobalCacheContext
    {
        void Add<T>(string key, T value) where T : class;
        T Get<T>(string key) where T : class;
        void Delete(string key);
        void Clear(string prefix = "");

    }
}