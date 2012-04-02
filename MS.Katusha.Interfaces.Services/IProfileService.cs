using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IProfileService
    {
        IEnumerable<Profile> GetNewProfiles(Expression<Func<Profile, bool>> controllerFilter, out int total, int pageNo = 1, int pageSize = 20);

        long GetProfileId(Guid guid);
        long GetProfileId(string friendlyName);

        Profile GetProfile(long profileId, Profile visitorProfile = null, params Expression<Func<Profile, object>>[] includeExpressionParams);

        void CreateProfile(Profile profile);
        void DeleteProfile(long profileId, bool force = false);
        void UpdateProfile(Profile profile);

        void DeleteCountriesToVisit(long profileId, Country country);
        void AddCountriesToVisit(long profileId, Country country);
        void DeleteLanguagesSpoken(long profileId, Language language);
        void AddLanguagesSpoken(long profileId, Language language);
        void DeleteSearches(long profileId, LookingFor lookingFor);
        void AddSearches(long profileId, LookingFor lookingFor);

        void MakeProfilePhoto(long profileId, Guid photoGuid);
        void DeletePhoto(long profileId, Guid guid, string pathToPhotos);
        ViewDataUploadFilesResult AddPhoto(long profileId, string description, string pathToPhotos, HttpPostedFileBase hpf);
        void AddSamplePhoto(long profileId, string description, string pathToPhotos, string fileName, string filePath);

        IEnumerable<Conversation> GetMessages(long profileId, out int total, int pageNo = 1, int pageSize = 20);
        void SendMessage(Conversation data);
        void ReadMessage(long id, Guid messageGuid);
        IEnumerable<Visit> GetVisitors(long profileId, out int total, int pageNo = 1, int pageSize = 20);
    }
}