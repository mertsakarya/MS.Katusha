using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class LanguagesSpokenRepositoryDB : BaseRepositoryDB<LanguagesSpoken>, ILanguagesSpokenRepositoryDB
    {
        public LanguagesSpokenRepositoryDB(KatushaDbContext dbContext) : base(dbContext) { }
    }
}