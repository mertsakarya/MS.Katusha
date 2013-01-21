using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using Raven.Client;

namespace MS.Katusha.Repositories.RavenDB
{
    public class VideoRoomRepositoryRavenDB : IVideoRoomRepository
    {
        private readonly IDocumentStore _documentStore;

        public VideoRoomRepositoryRavenDB(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public VideoRoom GetByRoomName(VideoRoomNames roomName)
        {
            using (var session = _documentStore.OpenSession()) {
                var t =  session.Load<VideoRoom>(roomName.ToString());
                return t;
            }
        }

        public void Add(VideoRoom room)
        {
            using (var session = _documentStore.OpenSession()) {
                session.Store(room, room.VideoRoomName.ToString());
                session.SaveChanges();
            }
        }
    }
}