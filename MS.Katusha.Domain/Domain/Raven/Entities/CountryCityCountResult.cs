namespace MS.Katusha.Domain.Raven.Entities
{
    public class CountryCityCountResult{
        public byte Gender { get; set; }
        public string CountryCode { get; set; }
        public int CityCode { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public int Count { get; set; }
    }
}