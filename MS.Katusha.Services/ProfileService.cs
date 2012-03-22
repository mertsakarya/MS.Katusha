using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Exceptions.Services;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Services
{
    public abstract class ProfileService<T> : IProfileService<T> where T : Profile
    {

        protected readonly IFriendlyNameRepository<T> _profileRepository;
        private readonly IUserRepositoryDB _userRepository;
        private readonly ICountriesToVisitRepositoryDB _countriesToVisitRepository;
        private readonly IPhotoRepositoryDB _photoRepository;
        private readonly ILanguagesSpokenRepositoryDB _languagesSpokenRepository;
        private readonly ISearchingForRepositoryDB _searchingForRepository;

        protected ProfileService(IFriendlyNameRepository<T> repository, IUserRepositoryDB userRepository, 
            ICountriesToVisitRepositoryDB countriesToVisitRepository, IPhotoRepositoryDB photoRepository,
            ILanguagesSpokenRepositoryDB languagesSpokenRepository, ISearchingForRepositoryDB searchingForRepository)
        {
            _profileRepository = repository;
            _userRepository = userRepository;
            _countriesToVisitRepository = countriesToVisitRepository;
            _photoRepository = photoRepository;
            _languagesSpokenRepository = languagesSpokenRepository;
            _searchingForRepository = searchingForRepository;
        }

        public IEnumerable<T> GetNewProfiles(int pageNo = 1, int pageSize = 20)
        {
            var items = _profileRepository.GetAll(pageNo, pageSize);
            return items;
        }

        public IEnumerable<T> GetMostVisitedProfiles(int pageNo = 1, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public virtual T GetProfile(Guid guid, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            var item = _profileRepository.GetByGuid(guid, includeExpressionParams);
            return item;
        }

        /// <summary>
        /// Tries to find friendlyName as Guid first, if unsuccessfull, looks for friendlyName
        /// </summary>
        /// <param name="friendlyName">This value can be Guid or a FriendlyName</param>
        /// <param name="includeExpressionParams"></param>
        /// <returns></returns>
        public T GetProfile(string friendlyName, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            Guid guid;
            if (Guid.TryParse(friendlyName, out guid))
                return GetProfile(guid, includeExpressionParams);
            var item = _profileRepository.GetByFriendlyName(friendlyName, includeExpressionParams);
            if (item == null)
                throw new KatushaFriendlyNameNotFoundException(friendlyName);
            return item;
        }

        public void CreateProfile(T profile)
        {
            if (!String.IsNullOrEmpty(profile.FriendlyName))
                if (_profileRepository.CheckIfFriendlyNameExists(profile.FriendlyName, profile.Id))
                    throw new KatushaFriendlyNameExistsException(profile as Profile);
            _profileRepository.Add(profile);
            _profileRepository.Save();
            var user = _userRepository.GetByGuid(profile.Guid);
            user.Gender = (profile is Boy) ? (byte) Sex.Male : (profile is Girl) ? (byte) Sex.Female : (byte) 0;
            _userRepository.FullUpdate(user);
        }

        public void DeleteProfile(Guid guid, bool force = false)
        {
            var profile = _profileRepository.GetByGuid(guid);
            if (profile != null)
            {
                ExecuteDeleteProfile(profile, force);
            }
        }

        private void ExecuteDeleteProfile(T profile, bool softDelete)
        {
            if (!softDelete)
                _profileRepository.Delete(profile);
            else
                _profileRepository.SoftDelete(profile);

            _profileRepository.Save();
            var user = _userRepository.GetByGuid(profile.Guid);
            user.Gender = (byte) 0;
            _userRepository.FullUpdate(user);
        }

        public void DeleteProfile(string friendlyName, bool softDelete = true)
        {
            var profile = _profileRepository.GetByFriendlyName(friendlyName);
            if (profile != null)
            {
                ExecuteDeleteProfile(profile, softDelete);
            }
        }

        public virtual void UpdateProfile(T profile)
        {
            if(profile != null && !String.IsNullOrEmpty(profile.FriendlyName))
                if (_profileRepository.CheckIfFriendlyNameExists(profile.FriendlyName, profile.Id))
                    throw new KatushaFriendlyNameExistsException(profile as Profile);
        }

        public IDictionary<string, string> GetCountriesToVisit() { 
            IResourceManager rm = new ResourceManager();
            return rm._L("Country");
        }

        public IList<string> GetSelectedCountriesToVisit(string friendlyName)
        {
            var profile = _profileRepository.GetByFriendlyName(friendlyName, p=>p.CountriesToVisit);
            var list = new List<string>();
            if (profile == null || profile.CountriesToVisit.Count == 0) return list;
            list.AddRange(profile.CountriesToVisit.Select(item => Enum.GetName(typeof (Country), item.Country)));
            return list;
        }

        public void DeleteCountriesToVisit(long profileId, Country country) { _countriesToVisitRepository.DeleteByProfileId(profileId, country); }

        public void AddCountriesToVisit(long profileId, Country country) { _countriesToVisitRepository.AddByProfileId(profileId, country); }
        public void DeleteLanguagesSpoken(long profileId, Language language) { _languagesSpokenRepository.DeleteByProfileId(profileId, language); }
        public void AddLanguagesSpoken(long profileId, Language language) { _languagesSpokenRepository.AddByProfileId(profileId, language); }
        public void DeleteSearches(long profileId, LookingFor lookingFor) { _searchingForRepository.DeleteByProfileId(profileId, lookingFor); }
        public void AddSearches(long profileId, LookingFor lookingFor) { _searchingForRepository.AddByProfileId(profileId, lookingFor); }

        protected void SetData(Profile dataProfile, Profile profile)
        {
            dataProfile.FriendlyName = profile.FriendlyName;
            dataProfile.Name = profile.Name; 
            dataProfile.Alcohol = profile.Alcohol;
            dataProfile.BirthYear = profile.BirthYear;
            dataProfile.BodyBuild = profile.BodyBuild;
            dataProfile.City = profile.City;
            dataProfile.Description = profile.Description;
            dataProfile.EyeColor = profile.EyeColor;
            dataProfile.From = profile.From;
            dataProfile.HairColor = profile.HairColor;
            dataProfile.Height = profile.Height;
            dataProfile.Name = profile.Name;
            dataProfile.Religion = profile.Religion;
            dataProfile.Smokes = profile.Smokes;
            dataProfile.ProfilePhotoGuid = profile.ProfilePhotoGuid;
        }


        public void DeletePhoto(Guid guid)
        {
            var entity = _photoRepository.GetByGuid(guid);
            if (entity != null) _photoRepository.Delete(entity);
        }

        public Photo GetPhotoByGuid(Guid guid)
        {
            return _photoRepository.GetByGuid(guid);
        }

        public void MakeProfilePhoto(Guid profileGuid, Guid photoGuid) {
            var profile = _profileRepository.GetByGuid(profileGuid);
            if (profile != null) {
                profile.ProfilePhotoGuid = photoGuid;
                _profileRepository.FullUpdate(profile); 
                _profileRepository.Save();
            }
 
        }

        public void AddPhoto(Photo photo)
        {
            var profile = _profileRepository.GetById(photo.ProfileId);
            if (profile == null) return;
            if(profile.ProfilePhotoGuid == Guid.Empty) { //set first photo as default photo
                profile.ProfilePhotoGuid = photo.Guid;
                _profileRepository.Save();
            }
            _photoRepository.Add(photo, photo.Guid);
            _photoRepository.Save();
            
        }
    }
}

