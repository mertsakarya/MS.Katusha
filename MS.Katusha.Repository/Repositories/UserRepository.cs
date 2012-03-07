using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class UserRepository : BaseGuidRepository<User>, IUserRepository
    {
        public UserRepository(KatushaDbContext dbContext) : base(dbContext) { }
    }
}