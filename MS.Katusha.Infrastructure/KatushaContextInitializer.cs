using System.Data.Entity;
using MS.Katusha.Domain;

namespace MS.Katusha.Infrastructure
{
    public class KatushaContextInitializer : DropCreateDatabaseIfModelChanges<KatushaDbContext> //CreateDatabaseIfNotExists<KatushaDbContext> //DropCreateDatabaseIfModelChanges<KatushaDbContext> //DropCreateDatabaseAlways<KatushaContext>// DropCreateDatabaseIfModelChanges<KatushaContext>
    {
        private KatushaDbContext _dbContext;
        protected override void Seed(KatushaDbContext dbContext)
        {
            _dbContext = dbContext;
            CreateData();
        }


        public void CreateData()
        {
            ReloadResources.Set(_dbContext);
        }

    }


}
