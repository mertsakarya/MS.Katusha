using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IVisitRepository : IGuidRepository<Visit>
    {
    }
    public interface IVisitRepositoryDB : IVisitRepository
    {
    }
    public interface IVisitRepositoryRavenDB : IVisitRepository
    {
    }
}
