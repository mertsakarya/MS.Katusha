using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IVisitRepository : IRepository<Visit>
    {
    }
    public interface IVisitRepositoryDB : IVisitRepository
    {
    }
    public interface IVisitRepositoryRavenDB : IVisitRepositoryDB, IRavenRepository<Visit>
    {
    }
}
