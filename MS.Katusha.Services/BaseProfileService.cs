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
    public abstract class BaseProfileService<T> : IProfileService<T> where T : Profile
    {

        protected readonly IFriendlyNameRepository<T> ProfileRepository;
        private readonly IUserRepositoryDB _userRepository;
        private readonly ICountriesToVisitRepositoryDB _countriesToVisitRepository;
        private readonly IPhotoRepositoryDB _photoRepository;
        private readonly ILanguagesSpokenRepositoryDB _languagesSpokenRepository;
        private readonly ISearchingForRepositoryDB _searchingForRepository;
        private readonly IConversationRepositoryDB _converstaionRepository;

        protected BaseProfileService(IFriendlyNameRepository<T> repository, IUserRepositoryDB userRepository, 
            ICountriesToVisitRepositoryDB countriesToVisitRepository, IPhotoRepositoryDB photoRepository,
            ILanguagesSpokenRepositoryDB languagesSpokenRepository, ISearchingForRepositoryDB searchingForRepository, IConversationRepositoryDB converstaionRepository)
        {
            ProfileRepository = repository;
            _userRepository = userRepository;
            _countriesToVisitRepository = countriesToVisitRepository;
            _photoRepository = photoRepository;
            _languagesSpokenRepository = languagesSpokenRepository;
            _searchingForRepository = searchingForRepository;
            _converstaionRepository = converstaionRepository;
        }

        public IEnumerable<T> GetNewProfiles(out int total, int pageNo = 1, int pageSize = 20)
        {
            var items = ProfileRepository.Query(p => p.Id > 0, pageNo, pageSize, out total, o => o.Id, p => p.Photos);
            return items;
        }

        public IEnumerable<T> GetMostVisitedProfiles(out int total, int pageNo = 1, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public long GetProfileId(string friendlyName)
        {
            return ProfileRepository.GetProfileIdByFriendlyName(friendlyName);
        }

        public long GetProfileId(Guid guid)
        {
            return ProfileRepository.GetProfileIdByGuid(guid);
        }

        public virtual T GetProfile(long profileId, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            var item = ProfileRepository.GetById(profileId, includeExpressionParams);
            return item;
        }

        public void CreateProfile(T profile)
        {
            if (!String.IsNullOrEmpty(profile.FriendlyName))
                if (ProfileRepository.CheckIfFriendlyNameExists(profile.FriendlyName, profile.Id))
                    throw new KatushaFriendlyNameExistsException(profile as Profile);
            ProfileRepository.Add(profile);
            ProfileRepository.Save();
        }

        public void DeleteProfile(long profileId, bool force = false)
        {
            var profile = ProfileRepository.GetById(profileId);
            if (profile != null)
            {
                ExecuteDeleteProfile(profile, force);
            }
        }

        private void ExecuteDeleteProfile(T profile, bool softDelete)
        {
            if (!softDelete) {
                ProfileRepository.Delete(profile);
            }  else {
                ProfileRepository.SoftDelete(profile);
            }
            ProfileRepository.Save();
            var user = _userRepository.GetByGuid(profile.Guid);
            user.Gender = (byte) 0;
            _userRepository.FullUpdate(user);
        }

        public virtual void UpdateProfile(T profile)
        {
            if(profile != null && !String.IsNullOrEmpty(profile.FriendlyName))
                if (ProfileRepository.CheckIfFriendlyNameExists(profile.FriendlyName, profile.Id))
                    throw new KatushaFriendlyNameExistsException(profile as Profile);
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
            dataProfile.Religion = profile.Religion;
            dataProfile.Smokes = profile.Smokes;
            dataProfile.ProfilePhotoGuid = profile.ProfilePhotoGuid;
        }

        public void DeletePhoto(long profileId, Guid photoGuid)
        {
            var entity = _photoRepository.GetByGuid(photoGuid);
            if (entity != null) _photoRepository.Delete(entity);
            var profile = ProfileRepository.GetById(profileId);
            if (profile != null)
                if (profile.ProfilePhotoGuid == photoGuid) {
                    profile.ProfilePhotoGuid = Guid.Empty;
                    ProfileRepository.FullUpdate(profile);
                    ProfileRepository.Save();
                }
        }

        public Photo GetPhotoByGuid(Guid photoGuid) //FROM DATABASE
        {
            return _photoRepository.GetByGuid(photoGuid);
        }

        public IEnumerable<Conversation> GetMessages(long profileId, out int total, int pageNo = 1, int pageSize = 20)
        {
            return _converstaionRepository.Query(q => q.FromId == profileId || q.ToId == profileId, pageNo, pageSize, out total, o => o.CreationDate, p=>p.From, p=>p.To).ToList();
        }
        
        public void SendMessage(Conversation message)
        {
            _converstaionRepository.Add(message);
            _converstaionRepository.Save();
        }

        public void MakeProfilePhoto(long profileId, Guid photoGuid) {
            var profile = ProfileRepository.GetById(profileId);
            if (profile != null) {
                profile.ProfilePhotoGuid = photoGuid;
                ProfileRepository.FullUpdate(profile); 
                ProfileRepository.Save();
            }
        }

        public void AddPhoto(Photo photo)
        {
            var profile = ProfileRepository.GetById(photo.ProfileId) as Profile;
            if (profile == null) return;
            if(profile.ProfilePhotoGuid == Guid.Empty) { //set first photo as default photo
                profile.ProfilePhotoGuid = photo.Guid;
                ProfileRepository.Save();
            }
            _photoRepository.Add(photo, photo.Guid);
            _photoRepository.Save();
            
        }
    }
}

