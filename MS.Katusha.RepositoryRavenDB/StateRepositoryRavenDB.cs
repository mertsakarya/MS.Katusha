using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Linq;

namespace MS.Katusha.Repositories.RavenDB
{
    public class StateRepositoryRavenDB : IStateRepositoryRavenDB
    {
        private readonly IDocumentStore _documentStore;

        public StateRepositoryRavenDB(IDocumentStore documentStore) {
            _documentStore = documentStore;
        }

        public State GetById(long profileId)
        {
            using (var session = _documentStore.OpenSession()) {
                return session.Query<State>().AsNoTracking().FirstOrDefault(p => p.ProfileId == profileId);
            }
        }

        public State Delete(State entity)
        {
            var name = String.Format("{0}s/{1}", typeof(State).Name.ToLower(CultureInfo.CreateSpecificCulture("en-US")), entity.Id);
            _documentStore.DatabaseCommands.Delete(name, null);
            return entity;
        }

        public IList<State> Query<TKey>(Expression<Func<State, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<State, TKey>> orderByClause, bool @ascending)
        {
            using (var session = _documentStore.OpenSession()) {
                RavenQueryStatistics stats;
                var q = session.Query<State>().Statistics(out stats).AsNoTracking();
                if (filter != null) q = q.Where(filter);
                if (orderByClause != null) q = (ascending) ? q.OrderBy(orderByClause) : q.OrderByDescending(orderByClause);
                var query  = q.Skip((pageNo - 1)*pageSize).Take(pageSize).ToList();
                total = stats.TotalResults;
                return query;
            }
        }

        public int Count(Expression<Func<State, bool>> filter) {
            using (var session = _documentStore.OpenSession()) {
                RavenQueryStatistics stats;
                var q = session.Query<State>().Statistics(out stats).AsNoTracking();
                if (filter != null) q = q.Where(filter);
                var a = q.Take(0).ToList();
                return stats.TotalResults;
            }

        }
        public DateTime UpdateStatus(long profileId, Sex gender)
        {
            var entity = new State {Id = profileId, ProfileId = profileId, Gender = (byte) gender, LastOnline = DateTime.Now};
            using (var session = _documentStore.OpenSession()) {
                session.Store(entity);
                session.SaveChanges();
            }
            return new DateTime(1900, 1, 1);
        }
    }
}