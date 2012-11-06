using System;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using System.Collections.Generic;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IStateRepositoryDB
    {
        State GetState(long profileId);
        void Update(State state);
        //DateTime UpdateStatus(Profile profile);
        IList<State> Query<TKey>(Expression<Func<State, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<State, TKey>> orderByClause, bool ascending);
        void Add(State state);
    }

    public interface IStateRepositoryRavenDB
    {
        State GetState(long profileId);
        void Delete(State state);
        void Update(State state);
        IList<State> Query<TKey>(Expression<Func<State, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<State, TKey>> orderByClause, bool ascending);
        IList<T> Search<T>(Expression<Func<T, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<T, object>> orderByClause, bool ascending = false);

        State GetState(Profile profile);
    }
}
