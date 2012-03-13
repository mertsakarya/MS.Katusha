using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Interfaces.Services
{
    public interface IProfileService<T> where T : BaseFriendlyModel
    {
        IEnumerable<T> GetNewProfiles(int pageNo = 1, int pageSize = 20);
        IEnumerable<T> GetMostVisitedProfiles(int pageNo = 1, int pageSize = 20);
        T GetProfile(int id, params Expression<Func<T, object>>[] includeExpressionParams);
        T GetProfile(Guid guid, params Expression<Func<T, object>>[] includeExpressionParams);
        void CreateProfile(T profile);
        void DeleteProfile(T profile);
        void UpdateProfile(T profile);
    }
}