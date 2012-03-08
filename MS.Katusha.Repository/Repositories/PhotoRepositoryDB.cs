using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class PhotoRepositoryDB : BaseGuidRepositoryDB<Photo>, IPhotoRepositoryDB
    {
        public PhotoRepositoryDB(KatushaDbContext dbContext) : base(dbContext) { }
    }
}