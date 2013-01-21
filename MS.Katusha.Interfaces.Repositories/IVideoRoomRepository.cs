using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IVideoRoomRepository
    {
        VideoRoom GetByRoomName(VideoRoomNames roomName);
        void Add(VideoRoom room);
    }
}
