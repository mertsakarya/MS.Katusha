using MS.Katusha.Domain.Entities;

namespace MS.Katusha.IRepositories.Interfaces
{
    public interface IGirlRepository : IFriendlyNameRepository<Girl>
    {
    }
    public interface IGirlRepositoryDB : IGirlRepository
    {
    }
    public interface IGirlRepositoryRavenDB : IGirlRepository
    {
    }
}
