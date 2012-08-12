using System;
using System.Collections.Generic;
using System.Data.Entity;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Domain
{
    public interface IKatushaDbContext
    {
        Database Database { get; }

        DbSet<Profile> Profiles { get; set; }
        DbSet<User> Users { get; set; }

        DbSet<Conversation> Conversations { get; set; }
        DbSet<State> States { get; set; }

        DbSet<SearchingFor> Searches { get; set; }
        DbSet<Photo> Photos { get; set; }
        DbSet<PhotoBackup> PhotoBackups { get; set; }

        DbSet<CountriesToVisit> CountriesToVisit { get; set; }
        DbSet<LanguagesSpoken> LanguagesSpoken { get; set; }

        DbSet<GeoCountry> GeoCountries { get; set; }
        DbSet<GeoLanguage> GeoLanguages { get; set; }
        DbSet<GeoName> GeoNames { get; set; }
        DbSet<GeoTimeZone> GeoTimeZones { get; set; }

        string DeleteProfile(Guid guid);
        void ExecuteNonQuery(List<string> commands);
    }
}