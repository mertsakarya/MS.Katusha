using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MS.Katusha.Domain.Entities
{
    public class GeoCountry
    {
        //#ISO	ISO3	ISO-Numeric	fips	Country	Capital	Area(in sq km)	Population	Continent	tld	CurrencyCode	CurrencyName	Phone	Postal Code Format	Postal Code Regex	Languages	geonameid	neighbours	EquivalentFipsCode

        [Key]
        public string ISO { get; set; } //ISO
        public string ISO3 { get; set; }
        public int ISONumeric { get; set; }
        public string FIPS { get; set; } //FIPS
        public string Country { get; set; }
        public string Capital { get; set; }
        public int Area { get; set; }
        public long Population { get; set; }
        public string Continent { get; set; }
        public string TLD { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string Phone { get; set; }
        public string PostalCodeFormat { get; set; }
        public string PostalCodeRegEx { get; set; }
        public string Languages { get; set; }
        public IList<string> LanguagesList { get { return Languages.Split(','); } }
        public int GeoNameId { get; set; }
        public string Neighbors { get; set; }
        public IList<string> NeighborsList { get { return Neighbors.Split(','); } }
        public string EquivalentFipsCode { get; set; }

        public new string ToString() { return Country; }
    }
}