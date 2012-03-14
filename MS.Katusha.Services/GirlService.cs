using System;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{

    public class GirlService : ProfileService<Girl>, IGirlService
    {

        public GirlService(IGirlRepositoryDB repository, IUserRepositoryDB userRepository)  : base(repository, userRepository)
        {
        }

        public override Girl GetProfile(Guid guid, params Expression<Func<Girl, object>>[] includeExpressionParams)
        {
            if (includeExpressionParams == null || includeExpressionParams.Length == 0)
            {
                includeExpressionParams = new Expression<Func<Girl, object>>[]
                                              {
                                                  p => p.CountriesToVisit, p => p.User, p => p.State,
                                                  p => p.LanguagesSpoken, p => p.LanguagesSpoken, p => p.Searches,
                                                  p => p.Photos
                                              };

            }
            var item = base.GetProfile(guid, includeExpressionParams);
            return item;
        }

        public override void UpdateProfile(Girl profile)
        {
            var dataProfile = base.GetProfile(profile.Guid);
            SetData(dataProfile, profile);
            dataProfile.BreastSize = profile.BreastSize;
            Repository.FullUpdate(dataProfile);
        }
    }
}
