using System;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{

    public class GirlService : ProfileService<Girl>, IGirlService
    {

        public GirlService(IGirlRepositoryDB repository, IUserRepositoryDB userRepository,
            ICountriesToVisitRepositoryDB countriesToVisitRepository, IPhotoRepositoryDB photoRepositoryDB,
            ILanguagesSpokenRepositoryDB languagesSpokenRepository, ISearchingForRepositoryDB searchingForRepository
            )
            : base(repository, userRepository, countriesToVisitRepository, photoRepositoryDB, languagesSpokenRepository, searchingForRepository) { }


        public override Girl GetProfile(long profileId, params Expression<Func<Girl, object>>[] includeExpressionParams)
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
            var item = base.GetProfile(profileId, includeExpressionParams);
            return item;
        }

        public override void UpdateProfile(Girl profile)
        {
            base.UpdateProfile(profile);
            var dataProfile = base.GetProfile(profile.Id);
            SetData(dataProfile, profile);
            dataProfile.BreastSize = profile.BreastSize;
            ProfileRepository.FullUpdate(dataProfile);
        }
    }
}
