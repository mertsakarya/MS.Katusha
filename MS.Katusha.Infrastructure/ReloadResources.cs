using System;
using System.Collections.Generic;
using System.Data.Objects;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Repositories.DB.Context;

namespace MS.Katusha.Infrastructure
{
    public static class ReloadResources
    {
        public static List<string> Reset(IKatushaDbContext dbContext)
        {
            Delete(dbContext);
            return Set(dbContext);
        }

        public static List<string> Set(IKatushaDbContext dbContext)
        {
            var parser = new ConfigParser(dbContext);
            return parser.Parse();
        }

        public static void Delete(IKatushaDbContext dbContext)
        {
            var context = dbContext as KatushaDbContext;
            if (context == null) return;
            foreach (var tableName in new string[] { "Resources", "ConfigurationDatas", "ResourceLookups", "GeoCountries", "GeoLanguages", "GeoNames", "GeoTimeZones" }) {

                dbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE [" + tableName + "]");
            }
            ClearLocal<Resource>(context);
            ClearLocal<ConfigurationData>(context);
            ClearLocal<ResourceLookup>(context);
            ClearLocal<GeoCountry>(context);
            ClearLocal<GeoLanguage>(context);
            ClearLocal<GeoName>(context);
            ClearLocal<GeoTimeZone>(context);
        }

        private static void ClearLocal<T>(KatushaDbContext context) where T : class
        {
            context.Set<T>().Local.Clear();
            foreach (var item in context.Set<T>().Local) 
                context.Set<T>().Local.Remove(item);
        }

        public static void ClearDatabase(IKatushaDbContext dbContext)
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
