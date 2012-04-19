using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;

namespace MS.Katusha.Repositories.DB
{
    public class StateRepositoryDB : IStateRepositoryDB
    {
        private readonly KatushaDbContext _dbContext;

        public StateRepositoryDB(IKatushaDbContext dbContext) {
            _dbContext = dbContext as KatushaDbContext;
        }

        public State GetById(long profileId) { return _dbContext.States.AsNoTracking().FirstOrDefault(p => p.ProfileId == profileId); }

        public State Delete(State state)
        {
            var entity = _dbContext.States.FirstOrDefault(p => p.Id == state.Id);
            if(entity != null)
                _dbContext.States.Remove(entity);
            return entity;
        }

        public DateTimeOffset UpdateStatus(long profileId, Sex gender)
        {
            var state = _dbContext.States.FirstOrDefault(p => p.ProfileId == profileId);
            DateTimeOffset retVal;
            if (state == null) {
                retVal = new DateTimeOffset(new DateTime(1900, 1, 1));
                _dbContext.States.Add(new State { Gender = (byte)gender, ProfileId = profileId, LastOnline = DateTimeOffset.UtcNow });
            } else {
                retVal = state.LastOnline;
                state.LastOnline = DateTimeOffset.UtcNow;
            }
            _dbContext.SaveChanges();
            return retVal;
        }

        public IQueryable<State> Query<TKey>(Expression<Func<State, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<State, TKey>> orderByClause, bool ascending)
        {
            var q = _dbContext.States.AsNoTracking().AsQueryable();
            if (filter != null) q = q.Where(filter);
            total = q.Count();
            if (orderByClause != null) q = (ascending) ? q.OrderBy(orderByClause) : q.OrderByDescending(orderByClause);
            return q.Skip((pageNo - 1) * pageSize).Take(pageSize);
        }

        public int Count(Expression<Func<State, bool>> filter)
        {
            var q = _dbContext.States.AsNoTracking().AsQueryable();
            if (filter != null) q = q.Where(filter);
            return q.Count();
        }
    }
}