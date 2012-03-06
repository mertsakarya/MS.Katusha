using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryRavenDB.Repositories
{
    public class PhotoRepository : BaseGuidRepository<Photo>, IPhotoRepository
    {
        public PhotoRepository(KatushaContext context) : base(context) { }
    }
}