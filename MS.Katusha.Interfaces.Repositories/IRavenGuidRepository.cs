using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IRavenGuidRepository<T> : IRavenRepository<T>, IGuidRepository<T> where T : BaseGuidModel
    {
    }
}