using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Base;

namespace MS.Katusha.Repositories.DB
{
    public class BoyRepositoryDB : BaseFriendlyNameRepositoryDB<Boy>, IBoyRepositoryDB
    {
        public BoyRepositoryDB(IKatushaDbContext dbContext) : base(dbContext)
        {
            

        }

    }

}
