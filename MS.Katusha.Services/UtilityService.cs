using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Repositories.RavenDB;

namespace MS.Katusha.Services
{
    public class UtilityService : IUtilityService
    {
        private readonly IKatushaRavenStore _ravenStore;
        private readonly IProfileRepositoryDB _profileRepository;
        private readonly IUserRepositoryDB _userRepository;
        private readonly IPhotoBackupService _photoBackupService;
        private readonly KatushaDbContext _dbContext;

        public UtilityService(IKatushaDbContext dbContext, IKatushaRavenStore ravenStore, IProfileRepositoryDB profileRepository, IUserRepositoryDB userRepository, IPhotoBackupService photoBackupService)
        {
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
            var extendedProfile = new ExtendedProfile() { Profile = profile, User = user, };
            foreach (var photo in profile.Photos) {
                extendedProfile.Images.Add(photo.Guid.ToString(), Convert.ToBase64String(_photoBackupService.GetPhotoData(photo.Guid)));
            }
            return extendedProfile;
        }
    }

}
