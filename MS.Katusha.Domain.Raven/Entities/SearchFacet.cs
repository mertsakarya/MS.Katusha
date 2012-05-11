namespace MS.Katusha.Domain.Raven.Entities
{
    public abstract class BaseSearchFacet
    {
        public byte Gender { get; set; }
        public int BirthYear { get; set; }
        public int Height { get; set; }
        public string CountryCode { get; set; }
        public int CityCode { get; set; }
        public byte BodyBuild { get; set; }
        public byte HairColor { get; set; }
        public byte EyeColor { get; set; }
    }
}