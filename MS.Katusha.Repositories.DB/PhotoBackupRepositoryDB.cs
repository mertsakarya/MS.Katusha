using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Base;

namespace MS.Katusha.Repositories.DB
{
    public class PhotoBackupRepositoryDB : BaseGuidRepositoryDB<PhotoBackup>, IPhotoBackupRepositoryDB
    {
        public PhotoBackupRepositoryDB(IKatushaDbContext dbContext) : base(dbContext) { }
    }
}