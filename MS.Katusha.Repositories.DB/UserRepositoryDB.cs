using System;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Base;
using MS.Katusha.Repositories.DB.Context;

namespace MS.Katusha.Repositories.DB
{
    public class UserRepositoryDB : BaseGuidRepositoryDB<User>, IUserRepositoryDB
    {
        public UserRepositoryDB(IKatushaDbContext dbContext) : base(dbContext) { }
        public User GetByUserName(string userName, params Expression<Func<User, object>>[] includeExpressionParams) { return Single(p => p.UserName == userName, includeExpressionParams); }
    }
}