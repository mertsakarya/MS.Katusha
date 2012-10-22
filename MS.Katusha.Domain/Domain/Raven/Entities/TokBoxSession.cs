using System;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class TokBoxSession
    {
        public Guid ProfileGuid { get; set; }
        public string SessionId { get; set; }
        public DateTime LastModified { get; set; }
    }
}
