using System;

namespace MS.Katusha.Infrastructure.Exceptions.Resources
{
    public class KatushaConfigurationException : KatushaException
    {
        public KatushaConfigurationException(string key, string value, Exception innerException)
            : base("Configuration", string.Format("Configuration Exception on {0} : {1}", key, value), innerException)
        {
        }
    }

}
