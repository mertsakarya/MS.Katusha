using System.Data.Entity;
using System.Linq;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;
using MS.Katusha.Domain;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class BoyRepositoryDB : BaseFriendlyNameRepositoryDB<Boy>, IBoyRepositoryDB
    {
        public BoyRepositoryDB(IKatushaDbContext dbContext) : base(dbContext)
        {
            

        }

    }

}
