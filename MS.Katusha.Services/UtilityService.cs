using System.Collections.Generic;
using System.IO;
using MS.Katusha.Domain;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;
using Raven.Client;

namespace MS.Katusha.Services
{
    public class UtilityService : IUtilityService
    {
        private readonly KatushaDbContext _dbContext;

        public UtilityService(IKatushaDbContext dbContext)
        {
            _dbContext = dbContext as KatushaDbContext;
        }

        public void ClearDatabase(string photosFolder) {
            ReloadResources.ClearDatabase(_dbContext);
            RavenHelper.ClearRaven();
            foreach(var fileName in Directory.EnumerateFiles(photosFolder)) {
                File.Delete(fileName);
            }
        }

        public void RegisterRaven()
        {
            RavenHelper.RegisterRaven();
        }
    }

}
