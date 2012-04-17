using System.Collections.Generic;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface IVisitService : IRestore<Visit>
    {
        void Visit(Profile visitorProfile, Profile profile);
        IEnumerable<Visit> GetVisitors(long profileId, out int total, int pageNo = 1, int pageSize = 20);
    }
}