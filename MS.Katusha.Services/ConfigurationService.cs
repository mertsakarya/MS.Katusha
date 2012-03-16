using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private KatushaDbContext _dbContext;

        public ConfigurationService(IKatushaDbContext dbContext)
        {
            _dbContext = dbContext as KatushaDbContext;
        }

        public string InitializeDB()
        {
            return "NOT IMPLEMENTED";
        }

        public string ResetDatabaseResources()
        {
            ReloadResources.DeleteResourceLookups(_dbContext);
            ReloadResources.SetResourceLookups(_dbContext);
            ReloadResources.DeleteResources(_dbContext);
            ReloadResources.SetResources(_dbContext);
            var resourceManager = new ResourceManager();
            resourceManager.LoadResourceFromDb(new ResourceRepositoryDB(_dbContext));
            resourceManager.LoadResourceLookupFromDb(new ResourceLookupRepositoryDB(_dbContext));
            return "DONE";
            
        }
    }

}
