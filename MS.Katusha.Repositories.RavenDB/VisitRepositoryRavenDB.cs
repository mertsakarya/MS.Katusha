using System;
using System.Collections.Generic;
using System.Linq;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Base;
using MS.Katusha.Repositories.RavenDB.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace MS.Katusha.Repositories.RavenDB
{
    public class VisitRepositoryRavenDB : BaseRepositoryRavenDB<Visit>, IVisitRepositoryRavenDB
    {

        public VisitRepositoryRavenDB(IKatushaRavenStore documentStore)
            : base(documentStore)
        {
        }

        public IList<UniqueVisitorsResult> GetVisitorsSinceLastVisit(long profileId, DateTime lastVisitTime)
        {
            using (var session = DocumentStore.OpenSession()) {
                try {
                    var lvt = lastVisitTime.ToLocalTime();
                    return Queryable.OrderByDescending(session.Query<UniqueVisitorsResult, UniqueVisitorsIndex>().Where(p => p.ProfileId == profileId && p.LastVisitTime > lvt), p => p.LastVisitTime).ToList();
                } catch(InvalidOperationException) {
                    return new List<UniqueVisitorsResult>();
                }
            }
        }

        public IList<UniqueVisitorsResult> GetVisitors(long profileId, out int total, int pageNo = 1, int pageSize = 20) {
            using (var session = DocumentStore.OpenSession()) {
                RavenQueryStatistics stats;
                var result = Queryable.OrderByDescending(session.Query<UniqueVisitorsResult, UniqueVisitorsIndex>().Statistics(out stats).Where(p => p.ProfileId == profileId), p => p.LastVisitTime).ToList();
                total = stats.TotalResults;
                return result;
            }
        }

        public IList<UniqueVisitorsResult> GetMyVisits(long profileId, out int total, int pageNo = 1, int pageSize = 20) {
            using (var session = DocumentStore.OpenSession()) {
                RavenQueryStatistics stats;
                var result = Queryable.OrderByDescending(session.Query<UniqueVisitorsResult, UniqueVisitorsIndex>().Statistics(out stats).Where(p => p.VisitorProfileId == profileId), p => p.LastVisitTime).ToList();
                total = stats.TotalResults;
                return result;
            }
        }
    }
}