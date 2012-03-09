using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IBoyRepository : IFriendlyNameRepository<Boy>
    {
    }
    public interface IBoyRepositoryDB : IBoyRepository
    {
    }
    public interface IBoyRepositoryRavenDB : IBoyRepository
    {
    }
}
