using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryRavenDB.Repositories
{
    public class LanguagesSpokenRepository : BaseRepository<LanguagesSpoken>, ILanguagesSpokenRepository
    {
        public LanguagesSpokenRepository(KatushaDbContext dbContext) : base(dbContext) { }
    }
}