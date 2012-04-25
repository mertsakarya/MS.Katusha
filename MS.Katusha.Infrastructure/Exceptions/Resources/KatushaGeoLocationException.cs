using System;

namespace MS.Katusha.Infrastructure.Exceptions.Resources
{
    public class KatushaGeoLocationException : KatushaException
    {
        public KatushaGeoLocationException(string key, string name, Exception innerException)
            : base("GeoLocation", string.Format("GeoLocation Exception on {0} : {1}", key, name), innerException)
        {
        }
    }

}
