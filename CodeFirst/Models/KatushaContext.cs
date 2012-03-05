using System.Data.Entity;

namespace CodeFirst.Models
{
    public class KatushaContext : DbContext
    {
        
        public DbSet<Girl> Girls { get; set; }
        public DbSet<Boy> Boys { get; set; }

        //public DbSet<Country> Countries { get; set; }
        //public DbSet<Language> Languages { get; set; }
        //public DbSet<Status> Statuses { get; set; }
        //public DbSet<MembershipType> MembershipTypes { get; set; }
        //public DbSet<Existance> Existances { get; set; }
        //public DbSet<BreastSize> BreastSizes { get; set; }
        //public DbSet<DickSize> DickSizes { get; set; }
        //public DbSet<DickThickness> DickThicknesses { get; set; }
    }
}