using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IPhotoRepositoryDB : IGuidRepository<Photo>
    {
    }
    public interface IPhotoRepositoryRavenDB : IPhotoRepositoryDB, IRavenGuidRepository<Photo>
    {
    }

}
