using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MS.Katusha.Domain.Entities
{
    public class GeoName
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
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

        public new string ToString() { return Name; }

    }
}