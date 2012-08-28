using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Base;
using MS.Katusha.Repositories.DB.Context;

namespace MS.Katusha.Repositories.DB
{
    public class UserRepositoryDB : BaseGuidRepositoryDB<User>, IUserRepositoryDB
    {
        public UserRepositoryDB(IKatushaDbContext dbContext) : base(dbContext) { }
    }
}