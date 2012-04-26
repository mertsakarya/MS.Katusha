using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Entities
{
    public class GeoLanguage
    {
        //ISO 639-3	ISO 639-2	ISO 639-1	Language Name
        public string ISO639_3 { get; set; }
        public string ISO639_2 { get; set; }
        public string ISO639_1 { get; set; }
        [Key]
        public string LanguageName { get; set; }

        public string ToString() { return LanguageName; }
    }

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

        public string ToString() { return Country; }
    }

    public class GeoName
    {
        [Key]
        public int GeoNameId { get; set; }
        [StringLength(200)]
        public string Name { get; set; } // name of geographical point (utf8) varchar(200)
        [StringLength(200)]
        public string AsciiName { get; set; } // name of geographical point in plain ascii characters, varchar(200)
        [NotMapped]
        public IList<string> AlternateNamesList { get { return AlternateNames.Split(','); } } //alternatenames, comma separated varchar(5000)
        public string AlternateNames { get; set; } //alternatenames, comma separated varchar(5000)
        public double Latitude { get; set; } //latitude in decimal degrees (wgs84)
        public double Longitude { get; set; } //longitude in decimal degrees (wgs84)
        [StringLength(1)]
        public string FeatureClass { get; set; } //see http://www.geonames.org/export/codes.html, char(1)
        [StringLength(10)]
        public string FeatureCode { get; set; } //see http://www.geonames.org/export/codes.html, varchar(10)
        [StringLength(2)]
        public string CountryCode { get; set; } //ISO-3166 2-letter country code, 2 characters
        [NotMapped]
        public IList<string> AlternateCountryCodesList { get { return CC2.Split(','); } } //alternate country codes, comma separated, ISO-3166 2-letter country code, 60 characters
        [StringLength(60)]
        public string CC2 { get; set; } //alternate country codes, comma separated, ISO-3166 2-letter country code, 60 characters
        [StringLength(20)]
        public string Admin1code { get; set; } //fipscode (subject to change to iso code), see exceptions below, see file admin1Codes.txt for display names of this code; varchar(20)
        [StringLength(80)]
        public string Admin2code { get; set; } //code for the second administrative division, a county in the US, see file admin2Codes.txt; varchar(80) 
        [StringLength(20)]
        public string Admin3code { get; set; } //code for third level administrative division, varchar(20)
        [StringLength(20)]
        public string Admin4code { get; set; } //code for fourth level administrative division, varchar(20)
        public long Population { get; set; } //bigint (8 byte int) 
        public int Elevation { get; set; } //in meters, integer
        public string DEM { get; set; } //digital elevation model, srtm3 or gtopo30, average elevation of 3''x3'' (ca 90mx90m) or 30''x30'' (ca 900mx900m) area in meters, integer. srtm processed by cgiar/ciat.
        [StringLength(40)]
        public string TimeZone { get; set; } //the timezone id (see file timeZone.txt) varchar(40)
        public string ModificationDate { get; set; } //date of last modification in yyyy-MM-dd format

        public string ToString() { return Name; }

    }

    public class GeoTimeZone
    {
        [Key]
        public string TimeZoneId { get; set; }
        public double GMTOffset { get; set; }
        public double DSTOffset { get; set; }
        public double RawOffset { get; set; }

        public string ToString() { return TimeZoneId; }
    }
}
