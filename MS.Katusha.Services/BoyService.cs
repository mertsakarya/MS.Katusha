using System;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{

    public class BoyService : BaseProfileService<Boy>, IBoyService
    {
        public BoyService(IBoyRepositoryDB repository, IUserRepositoryDB userRepository, 
            ICountriesToVisitRepositoryDB countriesToVisitRepository, IPhotoRepositoryDB photoRepositoryDB,
            ILanguagesSpokenRepositoryDB languagesSpokenRepository, ISearchingForRepositoryDB searchingForRepository, IConversationRepositoryDB converstaionRepository
            ) 
            : base(repository, userRepository, countriesToVisitRepository, photoRepositoryDB, languagesSpokenRepository, searchingForRepository, converstaionRepository) { }

        public override Boy GetProfile(long profileId, params Expression<Func<Boy, object>>[] includeExpressionParams)
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
            var item = base.GetProfile(profileId, includeExpressionParams);
            return item;
        }

        public override void UpdateProfile(Boy profile)
        {
            base.UpdateProfile(profile);
            var dataProfile = base.GetProfile(profile.Id);
            base.SetData(dataProfile, profile);
            dataProfile.DickSize = profile.DickSize;
            dataProfile.DickThickness = profile.DickThickness;
            ProfileRepository.FullUpdate(dataProfile);
        }
    }
}
