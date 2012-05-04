using System;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using System.Collections.Generic;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IStateRepositoryDB
    {
        DateTime UpdateStatus(Profile profile);
        IList<State> Query<TKey>(Expression<Func<State, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<State, TKey>> orderByClause, bool ascending);
    }

    public interface IStateRepositoryRavenDB
    {
        State GetById(long profileId);
        void Delete(State state);
        void UpdateStatus(State state);
        IList<State> Query<TKey>(Expression<Func<State, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<State, TKey>> orderByClause, bool ascending);
        IList<T> Search<T>(Expression<Func<T, bool>> filter, int pageNo, int pageSize, out int total);
    }
}
