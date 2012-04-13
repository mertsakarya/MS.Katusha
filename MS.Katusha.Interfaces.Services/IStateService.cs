using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IStateService
    {
        void Ping(long profileId, Sex gender);
        bool IsOnline(long profileId);
        long OnlineUsersCount();
        long OnlineGirlsCount();
        long OnlineMenCount();
        IEnumerable<State> OnlineGirls(out int total, int pageNo = 1, int pageSize = 20);
        IEnumerable<State> OnlineMen(out int total, int pageNo = 1, int pageSize = 20);
    }
}
