using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Interfaces.Services.Models;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Repositories.RavenDB;
using Raven.Abstractions.Commands;
using AutoMapper;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;
using PhotoBackup = MS.Katusha.Domain.Entities.PhotoBackup;
using Profile = MS.Katusha.Domain.Entities.Profile;

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
        private readonly IConversationService _conversationService;
        private readonly IPhotosService _photoService;
        private readonly IPhotoRepositoryDB _photoRepository;
        private readonly IProfileRepositoryRavenDB _profileRepositoryRaven;
        private readonly IVisitRepositoryRavenDB _visitRepositoryRaven;
        private readonly IConversationRepositoryRavenDB _conversationRepositoryRaven;

        public UtilityService(IPhotosService photoService, IConversationService conversationService, IProfileService profileService, IKatushaDbContext dbContext, IKatushaRavenStore ravenStore,
                              IPhotoRepositoryDB photoRepository, IProfileRepositoryDB profileRepository, IUserRepositoryDB userRepository, IPhotoBackupService photoBackupService,
                              ICountriesToVisitRepositoryDB countriesToVisitRepository, ILanguagesSpokenRepositoryDB languagesSpokenRepository, ISearchingForRepositoryDB searchingForRepository,
                              IConversationRepositoryDB conversationRepository, IProfileRepositoryRavenDB profileRepositoryRaven, IVisitRepositoryRavenDB visitRepositoryRaven, IConversationRepositoryRavenDB conversationRepositoryRaven)
        {
            _conversationRepositoryRaven = conversationRepositoryRaven;
            _visitRepositoryRaven = visitRepositoryRaven;
            _profileRepositoryRaven = profileRepositoryRaven;
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


        public IExtendedProfile GetExtendedProfile(User katushaUser, long profileId)
        {
            var includeExpressionParams = new Expression<Func<Profile, object>>[] {
                p => p.Photos
            };
            var profile = _profileRepository.GetById(profileId, includeExpressionParams);
            var user = _userRepository.GetById(profile.UserId);

            IExtendedProfile extendedProfile = new ApiExtendedProfile {
                Profile = Mapper.Map<ApiProfile>(profile),
                User = Mapper.Map <ApiUser>(user),
                CountriesToVisit = _countriesToVisitRepository.Query(p => p.ProfileId == profileId, null, false).Select(ctv => ctv.Country).ToArray(),
                LanguagesSpoken = _languagesSpokenRepository.Query(p => p.ProfileId == profileId, null, false).Select(ls => ls.Language).ToArray(),
                Searches = _searchingForRepository.Query(p => p.ProfileId == profileId, null, false).Select(s => ((LookingFor) s.Search).ToString()).ToArray(),
            };
            if (((UserRole)katushaUser.UserRole & UserRole.Administrator) == 0) {
            } else {
                var photoBackups = new List<ApiPhotoBackup>(profile.Photos.Count);
                if (profile.Photos.Count > 0) {
                    photoBackups.AddRange(profile.Photos.Select(photo => Mapper.Map<ApiPhotoBackup>(_photoBackupService.GetPhoto(photo.Guid))));
                }
                var messages = Mapper.Map<IList<ApiConversation>>(_conversationRepository.Query(p => p.FromId == profileId || p.ToId == profileId, null, false, e => e.From, e => e.To).ToList());
                extendedProfile = new AdminExtendedProfile(extendedProfile) {
                    User = Mapper.Map<ApiAdminUser>(user), 
                    Messages =  messages,
                    PhotoBackups = photoBackups
                };
            }
            return extendedProfile;
        }

        public IList<string> SetExtendedProfile(AdminExtendedProfile extendedProfile)
        {
            var list = new List<string>();
            var userDb = _userRepository.SingleAttached(p => p.UserName == extendedProfile.User.UserName);
            if (userDb == null) {
                var user = Mapper.Map<User>(extendedProfile.User);
                if (user == null) {
                    list.Add("WRONG USER");
                    return list;
                }
                user.CreationDate = DateTime.Now;
                user.DeletionDate = new DateTime(1900, 1, 1);
                user.ModifiedDate = DateTime.Now;
                userDb = _userRepository.Add(user);
            } else {
                if (userDb.Guid != extendedProfile.User.Guid) {
                    userDb.Guid = userDb.Guid;
                }
                userDb.Email = extendedProfile.User.Email;
                userDb.EmailValidated = extendedProfile.User.EmailValidated;
                userDb.FacebookUid = extendedProfile.User.FacebookUid;
                userDb.PaypalPayerId = extendedProfile.User.PaypalPayerId;
                userDb.UserRole = (long)extendedProfile.User.UserRole;
                userDb.Phone = extendedProfile.User.Phone;
                userDb.UserName = extendedProfile.User.UserName;
                _userRepository.FullUpdate(userDb);
            }

            var profile = GetProfile(_profileService.GetProfileId(userDb.Guid), userDb, extendedProfile);
            foreach (var photo in profile.Photos) SetDatetime(photo);
            if (profile.Id == 0) {
                if (!String.IsNullOrEmpty(extendedProfile.Profile.FriendlyName))
                    if (_profileRepository.CheckIfFriendlyNameExists(extendedProfile.Profile.FriendlyName))
                        extendedProfile.Profile.FriendlyName = "";
                SetDatetime(profile);
                profile = _profileService.CreateProfile(profile);
            } else {
                profile = _profileService.UpdateProfile(profile);
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
                    _photoBackupService.AddPhoto(Mapper.Map<PhotoBackup>(photoBackup));
                }
                list.AddRange(from photo in extendedProfile.Profile.Photos from suffix in PhotoTypes.Versions.Keys where _photoBackupService.GeneratePhoto(photo.Guid, (PhotoType) suffix) select "CREATED\t" + photo.Guid);
            }
            foreach (var message in Mapper.Map<IList<Conversation>>(extendedProfile.Messages)) {
                Guid otherGuid;
                if (message.ToGuid == extendedProfile.Profile.Guid) {
                    message.ToId = profile.Id;
                    otherGuid = message.FromGuid;
                    message.FromId = _profileService.GetProfileId(message.FromGuid);
                } else {
                    message.FromId = profile.Id;
                    otherGuid = message.ToGuid;
                    message.ToId = _profileService.GetProfileId(message.ToGuid);
                }
                var otherUser = _userRepository.GetByGuid(otherGuid);
                if (otherUser == null) continue;
                var messageGuid = message.Guid;
                var messageDb = _conversationRepository.SingleAttached(p => p.Guid == messageGuid);
                if (messageDb != null) {
                    _conversationService.SendMessage((message.FromId == profile.Id) ? userDb : otherUser, message);
                }
            }
            return list;
        }

        private static void SetDatetime(BaseModel baseModel)
        {
            baseModel.CreationDate = DateTime.Now;
            baseModel.DeletionDate = new DateTime(1900, 1, 1);
            baseModel.ModifiedDate = DateTime.Now;
        }

        private static Profile GetProfile(long profileId, User userDb, AdminExtendedProfile extendedProfile)
        {
            var profile = Mapper.Map<Profile>(extendedProfile.Profile);
            profile.UserId = userDb.Id;
            profile.Guid = userDb.Guid;
            profile.Id = profileId;
            return profile;
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
            if(photos.Count > 0)
                foreach (var item in photos)
                    _photoService.DeletePhoto(item.ProfileId, item.Guid);
            if (ravenCommands.Count > 0) _ravenStore.Batch(ravenCommands);
            if (sqlCommands.Count > 0) _dbContext.ExecuteNonQuery(sqlCommands);
        }
    }
}