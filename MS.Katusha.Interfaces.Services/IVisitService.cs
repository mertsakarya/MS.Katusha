using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface IVisitService : IRestore<Visit>
    {
        void Visit(Profile visitorProfile, Profile profile);
        IList<UniqueVisitorsResult> GetVisitors(long profileId, out int total, int pageNo = 1, int pageSize = 20);
        NewVisits GetVisitorsSinceLastVisit(long profileId, DateTime lastVisit);
    }
}