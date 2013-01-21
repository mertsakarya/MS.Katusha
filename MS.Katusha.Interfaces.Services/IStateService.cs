using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IStateService
    {
        PingResult Ping(Profile profile);
        IEnumerable<State> OnlineProfiles(byte sex, out int total, int pageNo = 1, int pageSize = 20);
        State GetState(Profile profile);
        IList<VideoRoom> GetVideoRooms(Profile profile);
    }

}
