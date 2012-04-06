namespace MS.Katusha.Domain.Raven.Entities
{
    public class ProfileSearchFacet : ProfileFacet
    {
        public byte Search { get; set; }
    }

    public class ProfileCountryFacet : ProfileFacet
    {
        public byte Country { get; set; }
    }

    public class ProfileLanguageFacet : ProfileFacet
    {
        public byte Language { get; set; }
    }
}