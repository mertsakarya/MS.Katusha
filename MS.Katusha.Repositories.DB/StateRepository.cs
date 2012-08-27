using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;

namespace MS.Katusha.Repositories.DB
{
    public class StateRepositoryDB : IStateRepositoryDB
    {
        private readonly KatushaDbContext _dbContext;

        public StateRepositoryDB(IKatushaDbContext dbContext) {
            _dbContext = dbContext as KatushaDbContext;
        }

        public DateTime UpdateStatus(Profile profile)
        {
            if (profile == null) return new DateTime(1900, 1, 1);
            var state = _dbContext.States.FirstOrDefault(p => p.ProfileId == profile.Id);
            DateTime retVal;
            if (state == null) {
                retVal = new DateTime(1900, 1, 1);
                _dbContext.States.Add(new State { Gender = profile.Gender, ProfileId = profile.Id, LastOnline = DateTime.Now });
            } else {
                retVal = state.LastOnline;
                state.LastOnline = DateTime.Now;
            }
            _dbContext.SaveChanges();
            return retVal;
        }

        public IList<State> Query<TKey>(Expression<Func<State, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<State, TKey>> orderByClause, bool ascending)
        {
            var q = _dbContext.States.AsNoTracking().AsQueryable();
            if (filter != null) q = q.Where(filter);
            total = q.Count();
            if (orderByClause != null) q = (ascending) ? q.OrderBy(orderByClause) : q.OrderByDescending(orderByClause);
            var query = q.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            return query;
        }
    }
}