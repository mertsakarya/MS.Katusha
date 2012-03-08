using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryDB.Repositories
{
    public class UserRepositoryDB : BaseGuidRepositoryDB<User>, IUserRepositoryDB
    {
        public UserRepositoryDB(IKatushaDbContext dbContext) : base(dbContext) { }
    }
}