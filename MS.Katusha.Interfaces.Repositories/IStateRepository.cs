using System;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using System.Collections.Generic;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IStateRepositoryDB
    {
        State GetById(long profileId);
        State Delete(State state);
        DateTime UpdateStatus(long profileId, Sex gender);
        IList<State> Query<TKey>(Expression<Func<State, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<State, TKey>> orderByClause, bool ascending);
        int Count(Expression<Func<State, bool>> filter);
    }

    public interface IStateRepositoryRavenDB : IStateRepositoryDB
    {
    }
}
