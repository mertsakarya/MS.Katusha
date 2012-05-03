using System;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Entities
{
    public class State : IdModel
    {
        public long ProfileId { get; set; }
        public byte Gender { get; set; }
        public DateTime LastOnline { get; set; }


        public int Birthyear { get; set; }
        public int Height { get; set; }
        public string From { get; set; }
        public string City { get; set; }
        public byte BodyBuild { get; set; }
        public byte HairColor { get; set; }
        public byte EyeColor { get; set; }
        public string CountriesToVisit { get; set; }
        public string Searches { get; set; }
        public bool HasPhoto { get; set; }
    }
}