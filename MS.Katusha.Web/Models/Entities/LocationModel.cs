using System;
using MS.Katusha.Infrastructure.Attributes;

namespace MS.Katusha.Web.Models.Entities
{
    public class LocationModel
    {
        public int CityCode { get; set; }

        [KatushaStringLength("Profile.City")]
        [KatushaField("Profile.City")]
        public string CityName { get; set; }

        public string CountryCode { get; set; }

        [KatushaField("Profile.From")]
        [KatushaRequired("Profile.From")]
        public string CountryName { get; set; }
        public override string ToString() { return ((!String.IsNullOrWhiteSpace(CountryCode) && CountryCode.Length == 2) ? CountryName : "") + ((CityCode <= 0) ? "" : ", " + CityName); }
    }
}