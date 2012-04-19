using System.Linq;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using Raven.Client.Indexes;

namespace MS.Katusha.Repositories.RavenDB.Indexes
{
    public class UniqueVisitorsIndex : AbstractIndexCreationTask<Visit, UniqueVisitorsResult>
    {
        public UniqueVisitorsIndex()
        {
            Map = docs => from doc in docs
                select new UniqueVisitorsResult {
                    ProfileId = doc.ProfileId,
                    VisitorProfileId = doc.VisitorProfileId,
                    Count = doc.VisitCount,
                    LastVisitTime = doc.ModifiedDate
                };
            Reduce = results => from result in results
                                group result by new {result.ProfileId, result.VisitorProfileId} into g
                                select new UniqueVisitorsResult {
                                    ProfileId = g.Key.ProfileId,
                                    VisitorProfileId = g.Key.VisitorProfileId,
                                    LastVisitTime = g.Max(x => x.LastVisitTime),
                                    Count = g.Sum(x => x.Count)
                                };
        }
    }
}