using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IProfileService<T> where T : BaseFriendlyModel
    {
        IEnumerable<T> GetNewProfiles(int pageNo = 1, int pageSize = 20);
        IEnumerable<T> GetMostVisitedProfiles(int pageNo = 1, int pageSize = 20);

        long GetProfileId(Guid guid);
        long GetProfileId(string friendlyName);

        T GetProfile(long profileId, params Expression<Func<T, object>>[] includeExpressionParams);
        
        void CreateProfile(T profile);
        void DeleteProfile(long profileId, bool force = false);
        void UpdateProfile(T profile);

        void DeleteCountriesToVisit(long profileId, Country country);
        void AddCountriesToVisit(long profileId, Country country);
        void DeleteLanguagesSpoken(long profileId, Language language);
        void AddLanguagesSpoken(long profileId, Language language);
        void DeleteSearches(long profileId, LookingFor lookingFor);
        void AddSearches(long profileId, LookingFor lookingFor);

        void MakeProfilePhoto(long profileId, Guid photoGuid);
        void DeletePhoto(long profileId, Guid guid);
        void AddPhoto(Photo photo);
        Photo GetPhotoByGuid(Guid guid);
    }
}