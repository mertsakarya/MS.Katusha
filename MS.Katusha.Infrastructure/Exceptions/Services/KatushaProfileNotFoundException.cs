namespace MS.Katusha.Infrastructure.Exceptions.Services
{
    public class KatushaProfileNotFoundException : KatushaException
    {
        public KatushaProfileNotFoundException(string key)
            : base("ProfileNotFound", null) { ProfileKey = key; }

        public string ProfileKey { get; private set; }
    }
}