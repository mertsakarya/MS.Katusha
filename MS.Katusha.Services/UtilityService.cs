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

namespace MS.Katusha.Services
{
    public class UtilityService : IUtilityService
    {
        private readonly IKatushaRavenStore _ravenStore;
        private readonly IProfileRepositoryDB _profileRepository;
        private readonly IUserRepositoryDB _userRepository;
        private readonly IPhotoBackupService _photoBackupService;
        private readonly KatushaDbContext _dbContext;
        private readonly ISearchingForRepositoryDB _searchingForRepository;
        private readonly ILanguagesSpokenRepositoryDB _languagesSpokenRepository;
        private readonly ICountriesToVisitRepositoryDB _countriesToVisitRepository;
        private IConversationRepositoryDB _conversationRepository;

        public UtilityService(IKatushaDbContext dbContext, IKatushaRavenStore ravenStore, IProfileRepositoryDB profileRepository, IUserRepositoryDB userRepository, IPhotoBackupService photoBackupService,
            ICountriesToVisitRepositoryDB countriesToVisitRepository, ILanguagesSpokenRepositoryDB languagesSpokenRepository, ISearchingForRepositoryDB searchingForRepository,
            IConversationRepositoryDB conversationRepository)
        {
            _conversationRepository = conversationRepository;
            _countriesToVisitRepository = countriesToVisitRepository;
            _languagesSpokenRepository = languagesSpokenRepository;
            _searchingForRepository = searchingForRepository;
            _ravenStore = ravenStore;
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _photoBackupService = photoBackupService;
            _dbContext = dbContext as KatushaDbContext;
        }

        public void ClearDatabase(string photosFolder) {
            ReloadResources.ClearDatabase(_dbContext);
            _ravenStore.ClearRaven();
            if (!Directory.Exists(photosFolder))
                Directory.CreateDirectory(photosFolder);
            else
                foreach(var fileName in Directory.EnumerateFiles(photosFolder)) 
                    File.Delete(fileName);
        }

        public void RegisterRaven()
        {
            _ravenStore.Create();
        }

        public IEnumerable<string> ResetDatabaseResources()
        {
            ReloadResources.Delete(_dbContext);
            var result = ReloadResources.Set(_dbContext);
            ResourceManager.LoadConfigurationDataFromDb(new ConfigurationDataRepositoryDB(_dbContext));
            ResourceManager.LoadResourceFromDb(new ResourceRepositoryDB(_dbContext));
            ResourceManager.LoadResourceLookupFromDb(new ResourceLookupRepositoryDB(_dbContext));
            ResourceManager.LoadGeoLocationDataFromDb(new GeoCountryRepositoryDB(_dbContext), new GeoLanguageRepositoryDB(_dbContext),  new GeoNameRepositoryDB(_dbContext), new GeoTimeZoneRepositoryDB(_dbContext));
            return result;
        }


        public ExtendedProfile GetExtendedProfile(long profileId)
        {
            var includeExpressionParams = new Expression<Func<Profile, object>>[] {
                p => p.Photos, 
                //p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches,
                //p => p.User,
                //p=> p.SentMessages, p=>p.RecievedMessages, p=>p.Visited, p=>p.WhoVisited
            };
            var profile = _profileRepository.GetById(profileId, includeExpressionParams);
            var user = _userRepository.GetById(profile.UserId);

            var extendedProfile = new ExtendedProfile { 
                Profile = profile, 
                User = user,
                CountriesToVisit = _countriesToVisitRepository.Query(p=>p.ProfileId == profileId, null, false).Select(ctv => ctv.Country).ToArray(),
                LanguagesSpoken = _languagesSpokenRepository.Query(p => p.ProfileId == profileId, null, false).Select(ls => ls.Language).ToArray(),
                Searches = _searchingForRepository.Query(p => p.ProfileId == profileId, null, false).Select(s => ((LookingFor)s.Search).ToString()).ToArray(),
            };
            foreach (var photo in profile.Photos) {
                extendedProfile.Images.Add(new KeyValuePair<string, string>(photo.Guid.ToString(), Convert.ToBase64String(_photoBackupService.GetPhotoData(photo.Guid))));
            }
            var list = _conversationRepository.Query(p => p.FromId == profileId || p.ToId == profileId, null, false, e=>e.From, e=>e.To).ToList();
            extendedProfile.Messages = AutoMapper.Mapper.Map<IList<Domain.Raven.Entities.Conversation>>(list).ToList();

            return extendedProfile;
        }
    }

}
