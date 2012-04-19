using System;
using System.Collections.Generic;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class NewVisits
    {
        public IList<UniqueVisitorsResult> Visits { get; set; }
        public DateTimeOffset LastVisitTime { get; set; }
    }
}