using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Repositories
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
