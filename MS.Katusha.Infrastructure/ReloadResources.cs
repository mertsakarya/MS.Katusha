using System.Linq;
using MS.Katusha.Domain;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Infrastructure
{
    public static class ReloadResources
    {
        public static void Reset(KatushaDbContext dbContext)
        {
            Delete(dbContext);
            Set(dbContext);
        }

        public static string[] Set(KatushaDbContext dbContext)
        {
            var parser = new ConfigParser(dbContext);
            return parser.Parse();
        }

        public static void Delete(KatushaDbContext dbContext)
        {
            DeleteResourceLookups(dbContext);
            DeleteResources(dbContext);
            DeleteConfigurationDatas(dbContext);
        }

        private static void DeleteResourceLookups(KatushaDbContext dbContext)
        {
            var repository = new ResourceLookupRepositoryDB(dbContext);
            foreach (var item in repository.GetAll().ToArray())
                repository.Delete(item);
            repository.Save();
        }

        private static void DeleteConfigurationDatas(KatushaDbContext dbContext)
        {
            var repository = new ConfigurationDataRepositoryDB(dbContext);
            foreach (var item in repository.GetAll().ToArray())
                repository.Delete(item);
            repository.Save();
        }

        private static void DeleteResources(KatushaDbContext dbContext)
        {
            var repository = new ResourceRepositoryDB(dbContext);
            foreach (var item in repository.GetAll().ToArray())
                repository.Delete(item);
            repository.Save();
        }
    }
}
