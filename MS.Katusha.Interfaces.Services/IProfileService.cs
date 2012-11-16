using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IProfileService : IRestore<Profile>
    {
        IEnumerable<Profile> GetNewProfiles(Expression<Func<Profile, bool>> controllerFilter, out int total, int pageNo = 1, int pageSize = 20);

        long GetProfileId(Guid guid);
        long GetProfileId(string friendlyName);

        /// <summary>
        /// Use this if you know profile.Id. MUCH FASTER!
        /// </summary>
        Profile GetProfile(long profileId, Profile visitorProfile = null, params Expression<Func<Profile, object>>[] includeExpressionParams);

        /// <summary>
        /// For logged in user only!!!
        /// </summary>
        Profile GetProfileDB(long profileId, params Expression<Func<Profile, object>>[] includeExpressionParams);

        /// <summary>
        /// Use this if you don't know profile.Id, but want to get by Guid or FriendlyName. SLOWER!
        /// </summary>
        Profile GetProfile(string key, Profile visitorProfile = null, params Expression<Func<Profile, object>>[] includeExpressionParams);

        Profile CreateProfile(Profile profile);
        void DeleteProfile(long profileId, bool force = false);
        Profile UpdateProfile(Profile profile);
        void UpdateRavenProfile(long id);


        void DeleteCountriesToVisit(long profileId, string country);
        void AddCountriesToVisit(long profileId, string country);
        void DeleteLanguagesSpoken(long profileId, string language);
        void AddLanguagesSpoken(long profileId, string language);
        void DeleteSearches(long profileId, LookingFor lookingFor);
        void AddSearches(long profileId, LookingFor lookingFor);

        IList<Guid> GetAllProfileGuids();
        IList<Profile> GetProfilesByTime(int page, DateTime date, out int total, int pageSize = 128);
    }
}