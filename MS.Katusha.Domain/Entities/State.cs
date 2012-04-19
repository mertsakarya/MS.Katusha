using System;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Entities
{
    public class State : IdModel
    {
        public long ProfileId { get; set; }
        public byte Gender { get; set; }
        public DateTimeOffset LastOnline { get; set; }
    }
}