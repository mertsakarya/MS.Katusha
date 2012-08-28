using System.ComponentModel.DataAnnotations;

namespace MS.Katusha.Domain.Entities
{
    public class GeoTimeZone
    {
        [Key]
        public string TimeZoneId { get; set; }
        public double GMTOffset { get; set; }
        public double DSTOffset { get; set; }
        public double RawOffset { get; set; }

        public new string ToString() { return TimeZoneId; }
    }
}
