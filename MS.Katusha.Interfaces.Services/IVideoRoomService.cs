using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IVideoRoomService
    {
        VideoRoom GetVideoRoom(VideoRoomNames roomName);
        IList<VideoRoom> GetRooms(Profile profile);
    }
}
