using System.Data.Entity;

namespace MS.Katusha.Repositories.DB.Context
{
    public class KatushaContextInitializer : CreateDatabaseIfNotExists<KatushaDbContext> //DropCreateDatabaseIfModelChanges<KatushaDbContext> //DropCreateDatabaseAlways<KatushaContext>// DropCreateDatabaseIfModelChanges<KatushaContext>
    {
        public KatushaContextInitializer() {
            
        }
        private KatushaDbContext _dbContext;

        protected override void Seed(KatushaDbContext dbContext)
        {
            _dbContext = dbContext;
            //CreateData();
        }


        //public void CreateData()
        //{
        //    ReloadResources.Set(_dbContext);
        //}

    }


}
