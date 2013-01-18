using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Base;
using Raven.Client;
using Raven.Client.Linq;

namespace MS.Katusha.Repositories.RavenDB
{
    public class ProfileRepositoryRavenDB : BaseFriendlyNameRepositoryRavenDB<Profile>, IProfileRepositoryRavenDB
    {
        public ProfileRepositoryRavenDB(IDocumentStore documentStore)
            : base(documentStore)
        { }

        public IDictionary<string, IEnumerable<FacetData>> FacetSearch<T>(Expression<Func<T, bool>> filter, string facetName)
        {
            var  dict = new Dictionary<string, IEnumerable<FacetData>>();
            using (var session = DocumentStore.OpenSession()) {
                var result = Queryable.Where(session.Query<T>(facetName + "Index"), filter).ToFacets("facets/" + facetName);
                if (result != null && result.Results != null) {
                    foreach (var r in result.Results) {
                        var key = r.Key;
                        var list = new List<FacetData>(r.Value.Values.Count);
                        list.AddRange(r.Value.Values.Select(value => new FacetData {Count = value.Hits, Range = value.Range}));
                        dict.Add(key, list);
                    }
                }
            }
            return dict;
        }

        public IList<T> Search<T>(Expression<Func<T, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<T, object>> orderByClause, bool ascending = false)
        {
            using (var session = DocumentStore.OpenSession()) {
                RavenQueryStatistics stats;
                var q = session.Query<T>().Statistics(out stats).Where(filter);
                if (orderByClause != null) q = (ascending) ? q.OrderBy(orderByClause) : q.OrderByDescending(orderByClause);
                var query = Queryable.Skip(q, (pageNo - 1) * pageSize).Take(pageSize).ToList();
                total = stats.TotalResults;
                return query;
            }
        }
    }
}
