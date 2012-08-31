using System;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IUserRepositoryDB : IGuidRepository<User>
    {
        User GetByUserName(string userName, params Expression<Func<User, object>>[] includeExpressionParams);
    }
}
