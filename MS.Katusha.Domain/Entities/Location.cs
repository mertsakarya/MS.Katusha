using System;
using System.ComponentModel.DataAnnotations;

namespace MS.Katusha.Domain.Entities
{
    [ComplexType]
    public class Location
    {
        public int CityCode { get; set; }
        [StringLength(200)]
        public string CityName { get; set; }
        [StringLength(2)]
        public string CountryCode { get; set; }
        [StringLength(200)]
        public string CountryName { get; set; }
        public override string ToString() { return ((!String.IsNullOrWhiteSpace(CountryCode) && CountryCode.Length == 2) ? CountryName : "") + ((CityCode <= 0) ? "" : ", " + CityName); }
    }
}