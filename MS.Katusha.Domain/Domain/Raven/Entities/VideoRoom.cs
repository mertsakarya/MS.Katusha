using MS.Katusha.Enumerations;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class VideoRoom
    {
        public VideoRoomNames VideoRoomName { get; set; }
        public string TokBoxSessionId { get; set; }

        public override string ToString()
        {
            return VideoRoomName.ToString();
        }
    }
}