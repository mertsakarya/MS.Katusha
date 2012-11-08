using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Infrastructure.Exceptions.Services;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IResourceService _resourceService;
        private readonly IVisitService _visitService;
        private readonly INotificationService _notificationService;
        private readonly IProfileRepositoryDB _profileRepository;
        private readonly IUserRepositoryDB _userRepository;
        private readonly ICountriesToVisitRepositoryDB _countriesToVisitRepository;
        private readonly ILanguagesSpokenRepositoryDB _languagesSpokenRepository;
        private readonly ISearchingForRepositoryDB _searchingForRepository;
        private readonly IProfileRepositoryRavenDB _profileRepositoryRaven;

        private readonly IKatushaGlobalCacheContext _katushaGlobalCache;

        public ProfileService(IResourceService resourceService, IVisitService visitService, INotificationService notificationService,  IProfileRepositoryDB profileRepository, IUserRepositoryDB userRepository,
            ICountriesToVisitRepositoryDB countriesToVisitRepository, ISearchingForRepositoryDB searchingForRepository,
            ILanguagesSpokenRepositoryDB languagesSpokenRepository, IProfileRepositoryRavenDB profileRepositoryRaven, 
            IKatushaGlobalCacheContext globalCacheContext)
        {
            _resourceService = resourceService;
            _visitService = visitService;
            _notificationService = notificationService;
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _countriesToVisitRepository = countriesToVisitRepository;
            _languagesSpokenRepository = languagesSpokenRepository;
            _searchingForRepository = searchingForRepository;
            _profileRepositoryRaven = profileRepositoryRaven;
            _katushaGlobalCache = globalCacheContext; 
        }


        public IEnumerable<Profile> GetNewProfiles(Expression<Func<Profile, bool>> filter, out int total, int pageNo = 1, int pageSize = 20)
        {
            var items = _profileRepositoryRaven.Query(filter, pageNo, pageSize, out total, o => o.CreationDate, false, p => p.Photos);
            return items;
        }

        public long GetProfileId(string friendlyName)
        {
            return _profileRepository.GetProfileIdByFriendlyName(friendlyName);
        }

        public long GetProfileId(Guid guid)
        {
            return _profileRepository.GetProfileIdByGuid(guid);
        }

        public virtual Profile GetProfile(long profileId, Profile visitorProfile = null, params Expression<Func<Profile, object>>[] includeExpressionParams)
        {
            var profile = _profileRepositoryRaven.GetById(profileId, includeExpressionParams);
            _visitService.Visit(visitorProfile, profile);
            return profile;
        }

        public virtual Profile GetProfileDB(long profileId, params Expression<Func<Profile, object>>[] includeExpressionParams)
        {
            if (includeExpressionParams == null || includeExpressionParams.Length == 0) {
                includeExpressionParams = new Expression<Func<Profile, object>>[] {
                    p => p.CountriesToVisit, p => p.User,
                    p => p.LanguagesSpoken, p => p.Searches,
                    p => p.Photos
                };
            }
            var profile = _profileRepository.GetById(profileId, includeExpressionParams);
            return profile;
        }

        public Profile GetProfile(string key, Profile visitorProfile = null, params Expression<Func<Profile, object>>[] includeExpressionParams)
        {
            Guid guid;
            var id = Guid.TryParse(key, out guid) ? GetProfileId(guid) : GetProfileId(key);
            return id > 0 ? GetProfile(id, visitorProfile, includeExpressionParams) : null;
        }

        public virtual Profile CreateProfile(Profile profile)
        {
            if (!String.IsNullOrEmpty(profile.FriendlyName))
                if (_profileRepository.CheckIfFriendlyNameExists(profile.FriendlyName, profile.Id))
                    throw new KatushaFriendlyNameExistsException(profile);
            if (!(profile.Gender == (byte)Sex.Male || profile.Gender == (byte)Sex.Female))
                throw new KatushaGenderNotExistsException(profile);
            _profileRepository.Add(profile);
            _profileRepository.Save();
            
            var user = _userRepository.SingleAttached(p => p.Id == profile.UserId);
            user.Gender = profile.Gender;
            _userRepository.FullUpdate(user);
            _katushaGlobalCache.Delete("U:" + user.UserName);

            UpdateRavenProfile(profile.Id);
            _notificationService.ProfileCreated(profile);
            return profile;
        }

        public void DeleteProfile(long profileId, bool force = false)
        {
            var profile = _profileRepository.SingleAttached(p=>p.Id == profileId);
            if (profile == null) return;
            var userId = profile.UserId;
            ExecuteDeleteProfile(profile, force);
            if (userId <= 0) return;
            var user = _userRepository.SingleAttached(p => p.Id == userId);
            if (user == null) return;
            user.Gender = 0;
            user.Guid = Guid.Empty;
            _userRepository.FullUpdate(user);
        }

        private void ExecuteDeleteProfile(Profile profile, bool softDelete)
        {
            if (!softDelete) {
                _profileRepository.Delete(profile);
            } else {
                _profileRepository.SoftDelete(profile);
            }
            var user = _userRepository.SingleAttached(p=> p.Guid == profile.Guid);
            user.Gender = 0;
            _userRepository.FullUpdate(user);
            _katushaGlobalCache.Delete("P:"+profile.Guid.ToString());
            _profileRepositoryRaven.Delete(profile);
        }

        public virtual Profile UpdateProfile(Profile profile)
        {
            if(profile == null)
                throw new ArgumentNullException("profile");
            if (!String.IsNullOrEmpty(profile.FriendlyName))
                if (_profileRepository.CheckIfFriendlyNameExists(profile.FriendlyName, profile.Id))
                    throw new KatushaFriendlyNameExistsException(profile);
            if (!(profile.Gender == (byte)Sex.Male || profile.Gender == (byte)Sex.Female))
                throw new KatushaGenderNotExistsException(profile);
            var cityName = _resourceService.GetLookupText("City", profile.Location.CityCode.ToString(CultureInfo.InvariantCulture), profile.Location.CountryCode);
            var countryName = _resourceService.GetLookupText("Country", profile.Location.CountryCode);
            var dataProfile = _profileRepository.SingleAttached(p => p.Id == profile.Id);
            dataProfile.FriendlyName = profile.FriendlyName;
            dataProfile.Name = profile.Name;
            dataProfile.Alcohol = profile.Alcohol;
            dataProfile.BirthYear = profile.BirthYear;
            dataProfile.BodyBuild = profile.BodyBuild;
            dataProfile.Location.CountryCode = profile.Location.CountryCode;
            dataProfile.Location.CityCode = profile.Location.CityCode;
            dataProfile.Location.CityName = cityName;
            dataProfile.Location.CountryName = countryName;
            dataProfile.Description = profile.Description;
            dataProfile.EyeColor = profile.EyeColor;
            dataProfile.HairColor = profile.HairColor;
            dataProfile.Height = profile.Height;
            dataProfile.Religion = profile.Religion;
            dataProfile.Smokes = profile.Smokes;
            dataProfile.ProfilePhotoGuid = profile.ProfilePhotoGuid;
            dataProfile.Gender = profile.Gender;
            dataProfile.DickSize = profile.DickSize;
            dataProfile.DickThickness = profile.DickThickness;
            dataProfile.BreastSize = profile.BreastSize; 

            _profileRepository.FullUpdate(dataProfile);

            UpdateRavenProfile(dataProfile.Id);
            return dataProfile;
        }

        public void UpdateRavenProfile(long id)
        {
            var profile = _profileRepository.GetById(id, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.User, p => p.Photos);
            _katushaGlobalCache.Delete("P:" + profile.Guid.ToString());
            _katushaGlobalCache.Delete("SearchableCountries"+profile.Gender.ToString(CultureInfo.InvariantCulture));
            _profileRepositoryRaven.FullUpdate(profile);
        }

        public void DeleteCountriesToVisit(long profileId, string country) { _countriesToVisitRepository.DeleteByProfileId(profileId, country); }

        public void AddCountriesToVisit(long profileId, string country) { _countriesToVisitRepository.AddByProfileId(profileId, country); }

        public void DeleteLanguagesSpoken(long profileId, string language) { _languagesSpokenRepository.DeleteByProfileId(profileId, language); }

        public void AddLanguagesSpoken(long profileId, string language) { _languagesSpokenRepository.AddByProfileId(profileId, language); }

        public void DeleteSearches(long profileId, LookingFor lookingFor) { _searchingForRepository.DeleteByProfileId(profileId, lookingFor); }

        public void AddSearches(long profileId, LookingFor lookingFor) { _searchingForRepository.AddByProfileId(profileId, lookingFor); }
        public IList<Guid> GetAllProfileGuids()
        {
            int total;
            var profiles = _profileRepository.GetAll(out total);
            var list = new List<Guid>(profiles.Count);
            list.AddRange(profiles.Select(item => item.Guid));
            return list;
        }

        public IList<Profile> GetProfilesByTime(DateTime date)
        {
            return _profileRepository.Query(p => p.ModifiedDate > date, null, false, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.User, p => p.Photos);
        }

        public IList<string> RestoreFromDB(Expression<Func<Profile, bool>> filter, bool deleteIfExists = false)
        {
            var result = new List<string>();
            var profiles = _profileRepository.Query(filter, null, false, p => p.Photos, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.User);
            foreach (var profile in profiles) {
                try {
                    if (deleteIfExists) {
                        var p = _profileRepositoryRaven.GetById(profile.Id);
                        if (p != null)
                            _profileRepositoryRaven.Delete(p);
                    }
                    _profileRepositoryRaven.Add(profile);
                } catch(Exception ex) {
                    result.Add(String.Format("{0} - {1}", profile.Id, ex.Message));
                }
            }
            return result;
        }
    }
}
