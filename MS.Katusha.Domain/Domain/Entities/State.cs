using System;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Entities
{
    public class State : IdModel
    {
        public long ProfileId { get; set; }
        public byte Gender { get; set; }
        public DateTime LastOnline { get; set; }

        public int BirthYear { get; set; }
        public int Height { get; set; }
        public string CountryCode { get; set; }
        public int CityCode { get; set; }
        public byte BodyBuild { get; set; }
        public byte HairColor { get; set; }
        public byte EyeColor { get; set; }
        public string CountriesToVisit { get; set; }
        public string Searches { get; set; }
        public bool HasPhoto { get; set; }

        public Guid ProfileGuid { get; set; }
        public string Name { get; set; }
        public Guid PhotoGuid { get; set; }
        public string TokBoxSessionId { get; set; }
        public string TokBoxTicketId { get; set; }
    }
}