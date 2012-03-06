using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryRavenDB.Repositories
{
    public class CountriesToVisitRepository : BaseRepository<CountriesToVisit>, ICountriesToVisitRepository
    {
        public CountriesToVisitRepository(KatushaContext context) : base(context) { }
    }
}