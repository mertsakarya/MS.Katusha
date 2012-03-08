using MS.Katusha.Domain.Entities;

namespace MS.Katusha.IRepositories.Interfaces
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
