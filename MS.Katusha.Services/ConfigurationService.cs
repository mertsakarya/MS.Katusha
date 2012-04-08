using System.Collections.Generic;
using MS.Katusha.Domain;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Services.Generators;

namespace MS.Katusha.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IUserService _userService;
        private readonly KatushaDbContext _dbContext;
        private readonly IProfileService _profileService;
        private readonly IPhotosService _photosService;

        public ConfigurationService(IKatushaDbContext dbContext, IUserService userService, IProfileService profileService, IPhotosService photosService)
        {
            _dbContext = dbContext as KatushaDbContext;
            _userService = userService;
            _profileService = profileService;
            _photosService = photosService;
        }

        public IEnumerable<string> ResetDatabaseResources()
        {
            ReloadResources.Delete(_dbContext);
            var result = ReloadResources.Set(_dbContext);
            var resourceManager = new ResourceManager();
            resourceManager.LoadConfigurationDataFromDb(new ConfigurationDataRepositoryDB(_dbContext));
            resourceManager.LoadResourceFromDb(new ResourceRepositoryDB(_dbContext));
            resourceManager.LoadResourceLookupFromDb(new ResourceLookupRepositoryDB(_dbContext));
            return result;
            
        }

        public void GenerateRandomUserAndProfile(int count)
        {
            var generator = new SampleGenerator(_profileService, _userService, _photosService);
            generator.CreateSamples(count);
        }
    }

}
