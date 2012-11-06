using System;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class _TokBoxSession
    {
        public string Id { get; set; }
        public Guid ProfileGuid { get; set; }
        public string Name { get; set; }
        public Guid PhotoGuid { get; set; }
        public byte Gender { get; set; }
        public string IP { get; set; }
        public string SessionId { get; set; }
        public DateTime LastModified { get; set; }
    }
}
