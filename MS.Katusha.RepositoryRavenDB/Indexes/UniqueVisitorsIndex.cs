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
                          select new {
                                         ProfileId = doc.VisitorProfileId,
                                         Count = doc.VisitCount
                                     };
            Reduce = results => from result in results
                                group result by result.ProfileId into g
                                select new {
                                               ProfileId = g.Key,
                                               Count = g.Sum(x => x.Count)
                                           };
        }
    }
}