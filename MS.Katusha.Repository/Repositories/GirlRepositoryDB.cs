using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class GirlRepositoryDB : BaseFriendlyNameRepositoryDB<Girl>, IGirlRepositoryDB
    {
        public GirlRepositoryDB(KatushaDbContext dbContext) : base(dbContext) { }
    }
}