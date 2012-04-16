using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IStateService
    {
        void Ping(long profileId, Sex gender);
        bool IsOnline(long profileId);
        IEnumerable<State> OnlineGirls(out int total, int pageNo = 1, int pageSize = 20);
        IEnumerable<State> OnlineMen(out int total, int pageNo = 1, int pageSize = 20);
        IEnumerable<State> OnlineProfiles(out int total, int pageNo = 1, int pageSize = 20);
    }
}
