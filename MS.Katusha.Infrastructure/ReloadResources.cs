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
            foreach (var tableName in new string[] {"Resources", "ConfigurationDatas", "ResourceLookups","GeoCountries", "GeoLanguages", "GeoNames", "GeoTimeZones" }) {
                dbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE [" + tableName + "]");
            }
        }
    }
}
