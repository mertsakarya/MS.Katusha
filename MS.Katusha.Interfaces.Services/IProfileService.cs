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
        //TODO: GUid'ler id olacak Id Guid veya FriendlyName olabilir. DB'ye GUid ve FriendName için Index yaratan kod yazýlmalý
        IEnumerable<T> GetNewProfiles(int pageNo = 1, int pageSize = 20);
        IEnumerable<T> GetMostVisitedProfiles(int pageNo = 1, int pageSize = 20);
        T GetProfile(Guid guid, params Expression<Func<T, object>>[] includeExpressionParams);
        T GetProfile(string friendlyName, params Expression<Func<T, object>>[] includeExpressionParams);
        void CreateProfile(T profile);
        void DeleteProfile(Guid guid, bool force = false);
        void DeleteProfile(string friendlyName, bool softDelete = true);
        void UpdateProfile(T profile);

        IDictionary<string, string> GetCountriesToVisit();
        IList<string> GetSelectedCountriesToVisit(string key);
        void  DeleteCountriesToVisit(long profileId, Country country);
        void AddCountriesToVisit(long profileId, Country country);
        void DeleteLanguagesSpoken(long profileId, Language language);
        void AddLanguagesSpoken(long profileId, Language language);
        void DeleteSearches(long profileId, LookingFor lookingFor);
        void AddSearches(long profileId, LookingFor lookingFor);

        void DeletePhoto(Guid guid);
        void AddPhoto(Photo photo);
        Photo GetPhotoByGuid(Guid guid);

        void MakeProfilePhoto(Guid guid, Guid guid_2);
    }
}