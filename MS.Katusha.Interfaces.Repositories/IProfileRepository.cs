using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IProfileRepository : IFriendlyNameRepository<Profile>
    {
    }
    public interface IProfileRepositoryDB : IProfileRepository
    {
    }
    public interface IProfileRepositoryRavenDB : IProfileRepository
    {
    }
}
