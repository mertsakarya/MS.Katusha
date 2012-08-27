using System;

namespace MS.Katusha.Domain.Service
{
    public class TicketData
    {
        public DateTime LoginDate { get; set; }
        public Guid Guid { get; set; }
        public long UserRole { get; set; }
    }
}