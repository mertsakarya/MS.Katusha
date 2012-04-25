using System.Collections.Generic;
using System.Linq;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Infrastructure
{
    public class GeoLocation
    {
        public GeoLocation() {
            Languages = new Dictionary<string, GeoLanguage>();
            Countries = new Dictionary<string, GeoCountry>();
            Names = new Dictionary<string, GeoName>();
            TimeZones = new Dictionary<string, GeoTimeZone>();
            Initialized = false;
        }

        public IDictionary<string, GeoLanguage> Languages { get; private set; }
        public IDictionary<string, GeoCountry> Countries { get; private set; }
        public IDictionary<string, GeoName> Names { get; private set; }
        public IDictionary<string, GeoTimeZone> TimeZones { get; private set; }
        public IDictionary<string, IList<GeoName>> CountryNames { get; private set; }
        public bool Initialized;
        public void Initialize()
        {
            CountryNames = new Dictionary<string, IList<GeoName>>(Countries.Count);
            foreach(var nameItem in Names) {
                var name = nameItem.Value;
                var key = name.CountryCode;
                IList<GeoName> list;
                if (!CountryNames.ContainsKey(key)) {
                    list = new List<GeoName>();
                    CountryNames.Add(key, list);
                } else list = CountryNames[key];
                list.Add(name);
            }
            Initialized = true;
        }

        public IList<string> GetNames(string countryCode)
        {
            var nameItems = CountryNames[countryCode];
            if (nameItems == null) return new List<string>();
            var list = new List<string>(nameItems.Count);
            list.AddRange(nameItems.Select(item => item.Name));
            return list;
        }

        public IDictionary<string, string> GetCountryNames()
        {
            var list = new Dictionary<string, string>(Countries.Count);
            foreach(var item in Countries) {list.Add(item.Key, item.Value.Country);}
            return list;
        }

        public IDictionary<string, string> GetLanguages()
        {
            var list = new Dictionary<string, string>();
            foreach(var country in Countries) {
                var languages = country.Value.LanguagesList;
                foreach(var language in languages) {
                    var languageDefinition = Languages[language];
                    list.Add(language, languageDefinition.LanguageName);
                }
            }
            return list;
        }

        public IDictionary<string, string> GetLanguages(string countryCode)
        {
            var country = Countries[countryCode];
            var list = new Dictionary<string, string>();
            var languages = country.LanguagesList;
            foreach (var language in languages) {
                var languageDefinition = Languages[language];
                list.Add(language, languageDefinition.LanguageName);
            }
            return list;
        }

        public IList<IList<string>> GetNamesWithAlternates(string countryCode)
        {
            var nameItems = CountryNames[countryCode];
            if (nameItems == null) return new List<IList<string>>();
            var list = new List<IList<string>>(nameItems.Count);
            foreach (var item in nameItems) {
                var items = new List<string> {item.Name};
                items.AddRange(item.AlternateNames.Split(','));
                list.Add(items);
            }
            return list;
        }
    }
}