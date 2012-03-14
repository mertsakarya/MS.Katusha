using System;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{

    public class BoyService : ProfileService<Boy>, IBoyService
    {
        public BoyService(IBoyRepositoryDB repository, IUserRepositoryDB userRepository) : base(repository, userRepository)
        {
        }

        public override Boy GetProfile(Guid guid, params Expression<Func<Boy, object>>[] includeExpressionParams)
        {
            if (includeExpressionParams == null || includeExpressionParams.Length == 0)
            {
                includeExpressionParams = new Expression<Func<Boy, object>>[]
                                              {
                                                  p => p.CountriesToVisit, p => p.User, p => p.State,
                                                  p => p.LanguagesSpoken, p => p.LanguagesSpoken, p => p.Searches,
                                                  p => p.Photos
                                              };

            } 
            var item = base.GetProfile(guid, includeExpressionParams);
            return item;
        }

        public override void UpdateProfile(Boy profile)
        {
            var dataProfile = base.GetProfile(profile.Guid);
            SetData(dataProfile, profile);
            dataProfile.DickSize = profile.DickSize;
            dataProfile.DickThickness = profile.DickThickness;
            Repository.FullUpdate(dataProfile);
        }
    }
}
