using System;

namespace MS.Katusha.Infrastructure.Attributes
{
    public class KatushaQuartzJobAttribute : Attribute
    {
        public double Interval { get; set; }
        public string Name { get; set; }
    }
}
