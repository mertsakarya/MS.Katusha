using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class VisitRepository : BaseGuidRepository<Visit>, IVisitRepository
    {
        public VisitRepository(KatushaDbContext dbContext) : base(dbContext) { }
    }
}