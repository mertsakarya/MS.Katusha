using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Repository.Interfaces;

namespace MS.Katusha.Repository.Repositories
{
    public class CountriesToVisitRepository : BaseRepository<CountriesToVisit>, ICountriesToVisitRepository
    {
        public CountriesToVisitRepository(KatushaContext context) : base(context) { }
    }
}