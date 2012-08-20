using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Text;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Domain
{
    public class KatushaDbContext : DbContext, IKatushaDbContext
    {
        public KatushaDbContext() {
            
        }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<State> States { get; set; }

        public DbSet<ConfigurationData> ConfigurationDatas { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceLookup> ResourceLookups { get; set; }

        public DbSet<SearchingFor> Searches { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<PhotoBackup> PhotoBackups { get; set; }
        public DbSet<CountriesToVisit> CountriesToVisit { get; set; }
        public DbSet<LanguagesSpoken> LanguagesSpoken { get; set; }

        public DbSet<GeoCountry> GeoCountries { get; set; }
        public DbSet<GeoLanguage> GeoLanguages { get; set; }
        public DbSet<GeoName> GeoNames { get; set; }
        public DbSet<GeoTimeZone> GeoTimeZones { get; set; }

        public string DeleteProfile(Guid guid)
        {
            return String.Format(KatushaDbSqlStatements.DeleteUserSql, guid);
        }

        public void ExecuteNonQuery(List<string> commands)
        {
            var sb = new StringBuilder();
            foreach (var command in commands)
                sb.AppendLine(command);
            using (var connection = new SqlConnection(Database.Connection.ConnectionString)) {
                connection.Open();
                using (var command = new SqlCommand(sb.ToString(), connection)) {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            //Database.ExecuteSqlCommand(statement);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>()
                        .HasMany(u => u.SentMessages)
                        .WithRequired(ul => ul.From)
                        .HasForeignKey(ul => ul.FromId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Profile>()
                        .HasMany(u => u.RecievedMessages)
                        .WithRequired(ul => ul.To)
                        .HasForeignKey(ul => ul.ToId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Profile>()
                        .HasMany(u => u.Visited)
                        .WithRequired(ul => ul.Profile)
                        .HasForeignKey(ul => ul.ProfileId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Profile>()
                        .HasMany(u => u.WhoVisited)
                        .WithRequired(ul => ul.VisitorProfile)
                        .HasForeignKey(ul => ul.VisitorProfileId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Profile>().HasRequired(x => x.User); //.WithOptional(s=>s.Profile); //.Map(x => x.MapKey("UserId"));

            base.OnModelCreating(modelBuilder);
        }
    }
}
