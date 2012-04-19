using System;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class UniqueVisitorsResult
    {
        public long ProfileId { get; set; }
        public long VisitorProfileId { get; set; }
        public int Count { get; set; }
        public DateTime LastVisitTime { get; set; }
    }
}