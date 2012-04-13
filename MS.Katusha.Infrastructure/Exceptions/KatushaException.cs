using System;

namespace MS.Katusha.Infrastructure.Exceptions
{
    public class KatushaException : Exception
    {
        public string Id { get; private set; }

        public KatushaException(string id, string message, Exception innerException)
            : base(message, innerException)
        {
            Id = id;
        }

        public KatushaException(string id, string message)
            : this(id, message, null)
        {
        }
    }
}
