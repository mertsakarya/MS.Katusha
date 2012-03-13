using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Services
{
    public abstract class ProfileService<T> : IProfileService<T> where T : BaseFriendlyModel
    {

        private readonly IFriendlyNameRepository<T> _repository;

        public ProfileService(IFriendlyNameRepository<T> repository)
        {
            _repository = repository;
        }

        public IEnumerable<T> GetNewProfiles(int pageNo = 1, int pageSize = 20)
        {
            var items = _repository.GetAll(pageNo, pageSize);
            return items;
        }

        public IEnumerable<T> GetMostVisitedProfiles(int pageNo = 1, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public T GetProfile(int id, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            throw new NotImplementedException();
        }

        public T GetProfile(Guid guid, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            var item = _repository.GetByGuid(guid, includeExpressionParams);
            return item;
        }

        public void CreateProfile(T profile)
        {
            _repository.Add(profile);
            _repository.Save();
        }

        public void DeleteProfile(T profile)
        {
            throw new NotImplementedException();
        }

        public void UpdateProfile(T profile)
        {
            throw new NotImplementedException();
        }
    }
}

