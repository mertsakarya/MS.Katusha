using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class GirlRepository : BaseFriendlyNameRepository<Girl>, IGirlRepository
    {
        public GirlRepository(KatushaDbContext dbContext) : base(dbContext) { }
    }
}