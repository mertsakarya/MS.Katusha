using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Repositories.RavenDB;
using Newtonsoft.Json;
using System.Web;
using Raven.Abstractions.Commands;

namespace MS.Katusha.Services
{
    public class UtilityService : IUtilityService
    {
        private readonly IKatushaRavenStore _ravenStore;
        private readonly IProfileRepositoryDB _profileRepository;
        private readonly IUserRepositoryDB _userRepository;
        private readonly IPhotoBackupService _photoBackupService;
        private readonly IKatushaDbContext _dbContext;
        private readonly ISearchingForRepositoryDB _searchingForRepository;
        private readonly ILanguagesSpokenRepositoryDB _languagesSpokenRepository;
        private readonly ICountriesToVisitRepositoryDB _countriesToVisitRepository;
        private readonly IConversationRepositoryDB _conversationRepository;
        private readonly IProfileService _profileService;
        private IConversationService _conversationService;
        private IPhotosService _photoService;
        private IPhotoRepositoryDB _photoRepository;
        private IVisitRepositoryDB _visitRepository;
        private IProfileRepositoryRavenDB _profileRepositoryRaven;
        private IVisitRepositoryRavenDB _visitRepositoryRaven;
        private IConversationRepositoryRavenDB _conversationRepositoryRaven;

        public UtilityService(IPhotosService photoService, IConversationService conversationService, IProfileService profileService, IKatushaDbContext dbContext, IKatushaRavenStore ravenStore,
                              IVisitRepositoryDB visitRepository, IPhotoRepositoryDB photoRepository, IProfileRepositoryDB profileRepository, IUserRepositoryDB userRepository, IPhotoBackupService photoBackupService,
                              ICountriesToVisitRepositoryDB countriesToVisitRepository, ILanguagesSpokenRepositoryDB languagesSpokenRepository, ISearchingForRepositoryDB searchingForRepository,
                              IConversationRepositoryDB conversationRepository, IProfileRepositoryRavenDB profileRepositoryRaven, IVisitRepositoryRavenDB visitRepositoryRaven, IConversationRepositoryRavenDB conversationRepositoryRaven)
        {
            _conversationRepositoryRaven = conversationRepositoryRaven;
            _visitRepositoryRaven = visitRepositoryRaven;
            _profileRepositoryRaven = profileRepositoryRaven;
            _visitRepository = visitRepository;
            _photoRepository = photoRepository;
            _photoService = photoService;
            _conversationService = conversationService;
            _profileService = profileService;
            _conversationRepository = conversationRepository;
            _countriesToVisitRepository = countriesToVisitRepository;
            _languagesSpokenRepository = languagesSpokenRepository;
            _searchingForRepository = searchingForRepository;
            _ravenStore = ravenStore;
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _photoBackupService = photoBackupService;
            _dbContext = dbContext; // as KatushaDbContext;
        }

        public void ClearDatabase(string photosFolder)
        {
            ReloadResources.ClearDatabase(_dbContext);
            _ravenStore.ClearRaven();
            if (!Directory.Exists(photosFolder))
                Directory.CreateDirectory(photosFolder);
            else
                foreach (var fileName in Directory.EnumerateFiles(photosFolder))
                    File.Delete(fileName);
        }

        public void RegisterRaven() { _ravenStore.Create(); }

        public IEnumerable<string> ResetDatabaseResources()
        {
            ReloadResources.Delete(_dbContext);
            var result = ReloadResources.Set(_dbContext);
            ResourceManager.LoadConfigurationDataFromDb(new ConfigurationDataRepositoryDB(_dbContext));
            ResourceManager.LoadResourceFromDb(new ResourceRepositoryDB(_dbContext));
            ResourceManager.LoadResourceLookupFromDb(new ResourceLookupRepositoryDB(_dbContext));
            ResourceManager.LoadGeoLocationDataFromDb(new GeoCountryRepositoryDB(_dbContext), new GeoLanguageRepositoryDB(_dbContext), new GeoNameRepositoryDB(_dbContext), new GeoTimeZoneRepositoryDB(_dbContext));
            return result;
        }


        public ExtendedProfile GetExtendedProfile(long profileId)
        {
            var includeExpressionParams = new Expression<Func<Profile, object>>[] {
                p => p.Photos, 
            };
            var profile = _profileRepository.GetById(profileId, includeExpressionParams);
            var user = _userRepository.GetById(profile.UserId);

            var extendedProfile = new ExtendedProfile {
                Profile = profile,
                User = user,
                CountriesToVisit = _countriesToVisitRepository.Query(p => p.ProfileId == profileId, null, false).Select(ctv => ctv.Country).ToArray(),
                LanguagesSpoken = _languagesSpokenRepository.Query(p => p.ProfileId == profileId, null, false).Select(ls => ls.Language).ToArray(),
                Searches = _searchingForRepository.Query(p => p.ProfileId == profileId, null, false).Select(s => ((LookingFor) s.Search).ToString()).ToArray(),
            };
            if (profile.Photos.Count > 0) {
                extendedProfile.PhotoBackups = new List<PhotoBackup>(profile.Photos.Count);
                foreach (var photo in profile.Photos) {
                    extendedProfile.PhotoBackups.Add(_photoBackupService.GetPhoto(photo.Guid));
                }
            }
            var messagesList = _conversationRepository.Query(p => p.FromId == profileId || p.ToId == profileId, null, false, e => e.From, e => e.To).ToList();
            extendedProfile.Messages = AutoMapper.Mapper.Map<IList<Domain.Raven.Entities.Conversation>>(messagesList).ToList();
            return extendedProfile;
        }

        public IList<string> SetExtendedProfile(ExtendedProfile extendedProfile)
        {
            var list = new List<string>();
            var userDb = _userRepository.SingleAttached(p => p.UserName == extendedProfile.User.UserName);
            if (userDb == null) {
                extendedProfile.User.Id = 0;
                userDb = _userRepository.Add(extendedProfile.User);
            } else {
                if (userDb.Guid != extendedProfile.User.Guid) {
                    userDb.Guid = userDb.Guid;
                }
                userDb.Email = extendedProfile.User.Email;
                userDb.EmailValidated = extendedProfile.User.EmailValidated;
                userDb.CreationDate = extendedProfile.User.CreationDate;
                userDb.FacebookUid = extendedProfile.User.FacebookUid;
                userDb.PaypalPayerId = extendedProfile.User.PaypalPayerId;
                userDb.UserRole = extendedProfile.User.UserRole;
                userDb.Phone = extendedProfile.User.Phone;
                userDb.UserName = extendedProfile.User.UserName;
                _userRepository.FullUpdate(userDb);
            }

            extendedProfile.Profile.UserId = userDb.Id;
            extendedProfile.Profile.Guid = userDb.Guid;
            var profileId = _profileService.GetProfileId(userDb.Guid);
            extendedProfile.Profile.Id = profileId;
            Profile profile;
            if (profileId == 0) {
                if (!String.IsNullOrEmpty(extendedProfile.Profile.FriendlyName))
                    if (_profileRepository.CheckIfFriendlyNameExists(extendedProfile.Profile.FriendlyName))
                        extendedProfile.Profile.FriendlyName = "";
                profile = _profileService.CreateProfile(extendedProfile.Profile);
            } else {
                extendedProfile.Profile.Id = profileId;
                profile = _profileService.UpdateProfile(extendedProfile.Profile);
            }
            if (extendedProfile.CountriesToVisit != null)
                foreach (var item in extendedProfile.CountriesToVisit)
                    _profileService.AddCountriesToVisit(profile.Id, item);
            if (extendedProfile.Searches != null) {
                LookingFor searches;
                foreach (var item in extendedProfile.Searches)
                    if (Enum.TryParse(item, out searches))
                        _profileService.AddSearches(profile.Id, searches);
            }
            if (extendedProfile.LanguagesSpoken != null)
                foreach (var item in extendedProfile.LanguagesSpoken)
                    _profileService.AddLanguagesSpoken(profile.Id, item);
            if (extendedProfile.PhotoBackups.Count > 0) {
                foreach (var photoBackup in extendedProfile.PhotoBackups) {
                    _photoBackupService.AddPhoto(photoBackup);
                }
                foreach (var photo in extendedProfile.Profile.Photos) {
                    foreach (var suffix in PhotoTypes.Versions.Keys) {
                        if (_photoBackupService.GeneratePhoto(photo.Guid, (PhotoType)suffix))
                            list.Add("CREATED\t" + photo.Guid);
                    }
                }
            }
            if (extendedProfile.PhotoBackups.Count > 0) {
                foreach (var photoBackup in extendedProfile.PhotoBackups) {
                    _photoBackupService.AddPhoto(photoBackup);
                }
                foreach (var photo in extendedProfile.Profile.Photos) {
                    foreach (var suffix in PhotoTypes.Versions.Keys) {
                        if (_photoBackupService.GeneratePhoto(photo.Guid, (PhotoType)suffix))
                            list.Add("CREATED\t" + photo.Guid);
                    }
                }
            }
            foreach (var message in extendedProfile.Messages) {
                Guid otherGuid;
                if (message.ToId == extendedProfile.Profile.Id) {
                    message.ToId = profile.Id;
                    otherGuid = message.FromGuid;
                } else {
                    message.FromId = profile.Id;
                    otherGuid = message.ToGuid;
                }
                var otherUser = _userRepository.GetByGuid(otherGuid);
                if (otherUser == null) continue;
                var messageDb = _conversationRepository.SingleAttached(p => p.Guid == message.Guid);
                if (messageDb != null) {
                    _conversationService.SendMessage((message.FromId == profile.Id) ? userDb : otherUser, message);
                }
            }
            return list;
        }

        public void DeleteProfile(Guid guid)
        {
            var profiles = _profileRepositoryRaven.Query(p => p.Guid == guid, null, false);
            if (profiles.Count == 0) return;
            var ravenCommands = new List<ICommandData>();
            var sqlCommands = new List<string>();
            var photos = new List<Photo>();

            foreach (var profile in profiles) {
                var profileId = profile.Id;
                var visits = _visitRepositoryRaven.Query(p => p.ProfileId == profileId || p.VisitorProfileId == profileId, null, false).ToArray();
                var messages = _conversationRepositoryRaven.Query(p => p.FromId == profileId || p.ToId == profileId, null, false).ToArray();
                photos = _photoRepository.Query(p => p.ProfileId == profileId, null, false).ToList();
                ravenCommands.AddRange(_ravenStore.DeleteProfile(profileId, visits, messages));
                sqlCommands.Add(_dbContext.DeleteProfile(profile.Guid));
            }
            if (ravenCommands.Count > 0) _ravenStore.Batch(ravenCommands);
            if (sqlCommands.Count > 0) _dbContext.ExecuteNonQuery(sqlCommands);
            if(photos.Count > 0) {
                foreach (var item in photos) {
                    _photoService.DeletePhoto(item.ProfileId, item.Guid);
                }
            }
        }
    }
}