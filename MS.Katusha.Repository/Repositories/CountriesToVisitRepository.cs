using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class CountriesToVisitRepository : BaseRepository<CountriesToVisit>, ICountriesToVisitRepository
    {
        public CountriesToVisitRepository(KatushaDbContext dbContext) : base(dbContext) { }
    }
}