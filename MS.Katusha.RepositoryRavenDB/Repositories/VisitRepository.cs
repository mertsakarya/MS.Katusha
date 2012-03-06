using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryRavenDB.Repositories
{
    public class VisitRepository : BaseGuidRepository<Visit>, IVisitRepository
    {
        public VisitRepository(string ravenDbConnectionString) : base(ravenDbConnectionString) { }
    }
}