using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IVisitRepository : IRepository<Visit>
    {
    }
    public interface IVisitRepositoryDB : IVisitRepository
    {
    }
    public interface IVisitRepositoryRavenDB : IVisitRepositoryDB, IRavenRepository<Visit>
    {
        IList<UniqueVisitorsResult> GetVisitorsSinceLastVisit(long profileId, DateTime lastVisitTime);
        IList<UniqueVisitorsResult> GetVisitors(long profileId, out int total, int pageNo = 1, int pageSize = 20);
        IList<UniqueVisitorsResult> GetMyVisits(long profileId, out int total, int pageNo = 1, int pageSize = 20);
    }
}
