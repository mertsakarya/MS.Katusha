using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class GridService<T> : IGridService<T> where T : BaseModel
    {
        private readonly IRepository<T> _repository;
        private readonly IKatushaGlobalCacheContext _globalCache;

        public GridService(IRepository<T> repository, IKatushaGlobalCacheContext globalCacheContext)
        {
            _repository = repository;
            _globalCache = globalCacheContext;
        }

        public IList<T> GetAll(out int total, int page, int pageSize) { return _repository.GetAll(out total, page, pageSize); }
        public void Add(T entity) { _repository.Add(entity); _repository.Save(); }
        T IGridService<T>.GetById(long id) { return _repository.GetById(id); }
        public void Update(T entity) { _repository.FullUpdate(entity); _repository.Save(); }
        public void Delete(T entity) { _repository.SoftDelete(entity); _repository.Save(); }
    }

    public class DetailGridRepositoryService<T> : GridService<T>, IDetailGridService<T> where T : BaseModel
    {
        private readonly IDetailRepository<T> _repository;
        protected DetailGridRepositoryService(IDetailRepository<T> repository, IKatushaGlobalCacheContext globalCacheContext) : base(repository, globalCacheContext) { _repository = repository; }
        public IList<T> GetAllByKey<TKey>(long id, out int total, int page, int pageSize, Expression<Func<T, TKey>> orderByClause, bool ascending) { return _repository.GetAllByKey<TKey>(id, out total, page, pageSize, orderByClause, ascending); }
    }
}
