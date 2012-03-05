using System.Data.Entity;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Domain
{
    public interface IKatushaContext
    {
        DbSet<Girl> Girls { get; set; }
        DbSet<Boy> Boys { get; set; }
        DbSet<User> Users { get; set; }

        DbSet<State> States { get; set; }
        DbSet<Conversation> Conversations { get; set; }

        DbSet<SearchingFor> Searches { get; set; }
        DbSet<Photo> Photos { get; set; }
        DbSet<CountriesToVisit> CountriesToVisit { get; set; }
        DbSet<LanguagesSpoken> LanguagesSpoken { get; set; }
    }
}