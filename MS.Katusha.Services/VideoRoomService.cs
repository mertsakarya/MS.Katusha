using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class VideoRoomService : IVideoRoomService
    {
        private readonly IVideoRoomRepository _videoRoomRepository;
        private readonly TokBox _tokBox;
        private bool _initialized;

        private static Dictionary<VideoRoomNames, VideoRoom> _videoRooms;
        private static IList<VideoRoom> _rooms;

        public VideoRoomService(IVideoRoomRepository videoRoomRepository)
        {
            _videoRoomRepository = videoRoomRepository;
            _initialized = false;
            _tokBox = new TokBox();
            if(_videoRooms == null) Initialize();
        }

        public VideoRoom GetVideoRoom(VideoRoomNames roomName)
        {
            if(!_initialized) Initialize();
            return _videoRooms[roomName];
        }

        public IList<VideoRoom> GetRooms(Profile profile)
        {
            return _rooms;
        }

        private void Initialize()
        {
            var roomNames = Enum.GetValues(typeof (VideoRoomNames));
            _videoRooms = new Dictionary<VideoRoomNames, VideoRoom>(roomNames.Length);
            _rooms = new List<VideoRoom>(roomNames.Length);
            foreach (VideoRoomNames roomName in roomNames) {
                var room = GetRoom(roomName);
                _videoRooms.Add(roomName, room);
                _rooms.Add(room);
            }
            _initialized = true;
        }

        private VideoRoom GetRoom(VideoRoomNames roomName)
        {
            var room = _videoRoomRepository.GetByRoomName(roomName);
            if (room == null) {
                room = new VideoRoom {
                    VideoRoomName = roomName,
                    TokBoxSessionId = _tokBox.CreateSession()
                };
                _videoRoomRepository.Add(room);
            }
            return room;
        }

    }
}