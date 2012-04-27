using System.Collections.Generic;
using System.Linq;
using MS.Katusha.Domain;
using MS.Katusha.Repositories.DB;
using Raven.Client;

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

        public static void ClearDatabase(KatushaDbContext dbContext)
        {

            foreach (var tableName in new string[] { "CountriesToVisits", "Conversations", "LanguagesSpokens", "SearchingFors", "Visits", "PhotoBackups", "Photos", "States" }) {

                dbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE [" + tableName + "]");
            }
            dbContext.Database.ExecuteSqlCommand("DELETE FROM [Profiles]");
            dbContext.Database.ExecuteSqlCommand("DELETE FROM [Users]");
            dbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT ([Users], RESEED, 1)");
            dbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT ([Profiles], RESEED, 1)");
        }
    }
}
