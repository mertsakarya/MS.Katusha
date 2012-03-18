using System;

namespace MS.Katusha.Exceptions.Resources
{
    public class KatushaResourceLookupException : KatushaException
    {
        public KatushaResourceLookupException(string lookupName, Exception innerException)
            : base("ResourceLookup", string.Format("Resource Lookup Exception on {0}", lookupName), innerException)
        {
        }
    }

}
