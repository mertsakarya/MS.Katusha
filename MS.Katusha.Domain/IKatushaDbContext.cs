using System.Data.Entity;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Domain
{
    public interface IKatushaDbContext
    {
        DbSet<Profile> Profiles { get; set; }
        DbSet<User> Users { get; set; }

        DbSet<Conversation> Conversations { get; set; }
        DbSet<State> States { get; set; }

        DbSet<SearchingFor> Searches { get; set; }
        DbSet<Photo> Photos { get; set; }
        DbSet<CountriesToVisit> CountriesToVisit { get; set; }
        DbSet<LanguagesSpoken> LanguagesSpoken { get; set; }
    }
}