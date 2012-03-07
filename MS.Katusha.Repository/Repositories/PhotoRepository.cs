using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class PhotoRepository : BaseGuidRepository<Photo>, IPhotoRepository
    {
        public PhotoRepository(KatushaDbContext dbContext) : base(dbContext) { }
    }
}