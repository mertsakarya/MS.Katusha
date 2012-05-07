using System.ComponentModel.DataAnnotations;

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

        public new string ToString() { return LanguageName; }
    }
}