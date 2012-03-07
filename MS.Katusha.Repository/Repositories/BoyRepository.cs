using System.Data.Entity;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class BoyRepository : BaseFriendlyNameRepository<Boy>, IBoyRepository
    {
        public BoyRepository(DbContext dbContext) : base(dbContext) { }
    }

}
