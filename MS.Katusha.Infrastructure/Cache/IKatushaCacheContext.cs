using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MS.Katusha.Infrastructure.Cache
{
    public interface IKatushaCacheContext
    {
        void Add<T>(string key, T value);
        T Get<T>(string key);
        void Delete(string key);
        bool Contains(string key);
        int Count(Expression<Func<CacheObject, bool>> expression);
        IList<CacheObject> Get(Expression<Func<CacheObject, bool>> expression);
    }
}