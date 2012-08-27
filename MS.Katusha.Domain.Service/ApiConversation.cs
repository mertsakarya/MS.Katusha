using System;

namespace MS.Katusha.Domain.Service
{
    public class ApiConversation
    {
        public string FromName { get; set; }
        public string ToName { get; set; }

        public Guid FromPhotoGuid { get; set; }
        public Guid ToPhotoGuid { get; set; }

        public Guid FromGuid { get; set; }
        public Guid ToGuid { get; set; }

        public string Message { get; set; }

        public string Subject { get; set; }

        public DateTime ReadDate { get; set; }
    }
}