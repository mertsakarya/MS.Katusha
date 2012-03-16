using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Infrastructure
{
    public class KatushaContextInitializer : DropCreateDatabaseIfModelChanges<KatushaDbContext> //DropCreateDatabaseAlways<KatushaContext>// DropCreateDatabaseIfModelChanges<KatushaContext>
    {
        private KatushaDbContext _dbContext;
        protected override void Seed(KatushaDbContext dbContext)
        {
            _dbContext = dbContext;
            CreateData();
        }


        public void CreateData()
        {
            ReloadResources.SetResources(_dbContext);
            ReloadResources.SetResourceLookups(_dbContext);
        }

    }


}
