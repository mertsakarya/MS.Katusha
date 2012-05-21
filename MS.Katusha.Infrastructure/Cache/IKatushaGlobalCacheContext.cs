namespace MS.Katusha.Infrastructure.Cache
{
    public interface IKatushaGlobalCacheContext
    {
        void Add<T>(string key, T value);
        T Get<T>(string key);
        void Delete(string key);
    }
}