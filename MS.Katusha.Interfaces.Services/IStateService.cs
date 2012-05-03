using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface IStateService : IRestore<State>
    {
        PingResult Ping(Profile profile);
        IEnumerable<State> OnlineProfiles(byte sex, out int total, int pageNo = 1, int pageSize = 20);
    }

}
