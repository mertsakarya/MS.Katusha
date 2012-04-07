using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IRavenFriendlyNameRepository<T> : IRavenGuidRepository<T>, IFriendlyNameRepository<T> where T : BaseFriendlyModel
    {
    }
}