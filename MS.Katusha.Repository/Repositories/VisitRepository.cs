using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Repository.Interfaces;

namespace MS.Katusha.Repository.Repositories
{
    public class VisitRepository : BaseGuidRepository<Visit>, IVisitRepository
    {
        public VisitRepository(KatushaContext context) : base(context) { }
    }
}