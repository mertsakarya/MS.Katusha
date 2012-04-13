using System;

namespace MS.Katusha.Infrastructure.Exceptions.Resources
{
    public class KatushaResourceException : KatushaException
    {
        public KatushaResourceException(string key, string value, Exception innerException)
            : base("Resource", string.Format("Resource Exception on {0} : {1}", key, value), innerException)
        {
        }
    }

}
