using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public IDictionary<string, IDictionary<string, string>> CountryCities { get; private set; }


        private IDictionary<string, string> _languageList;
        private IDictionary<string, string> _countryList;
        private IList<string> _cityListWithAlternates;
        private IList<string> _cityList;

        public bool Initialized;
        public void Initialize()
        {
            CountryNames = new Dictionary<string, IList<GeoName>>(Countries.Count);
            CountryCities = new Dictionary<string, IDictionary<string, string>>(Countries.Count);
            foreach (var nameItem in Names) {
                var name = nameItem.Value;
                var key = name.CountryCode.ToLowerInvariant();
                IList<GeoName> geoNames;
                IDictionary<string, string> cityNames;
                if (!CountryNames.ContainsKey(key)) {
                    geoNames = new List<GeoName>();
                    CountryNames.Add(key, geoNames);
                    cityNames = new Dictionary<string, string>();
                    CountryCities.Add(key, cityNames);
                } else {
                    geoNames = CountryNames[key];
                    cityNames = CountryCities[key];
                }
                geoNames.Add(name);
                if(!cityNames.ContainsKey(name.Name))
                    cityNames.Add(name.Name, name.Name);
            }
            var removeList = (from country in Countries where !CountryNames.ContainsKey(country.Key) select country.Key).ToList();
            foreach (var item in removeList) Countries.Remove(item);

            _languageList = new Dictionary<string, string>();
            foreach (var country in Countries) {
                var languages = country.Value.LanguagesList;
                var validLanguageList = new List<string>();
                foreach (var language in languages) {
                    if (language.Length == 5) {
                        validLanguageList.Add(language.Substring(0, 2).ToLowerInvariant());
                    } else if (language.Length == 2) {
                        validLanguageList.Add(language.ToLowerInvariant());
                    }
                }

                country.Value.Languages = string.Join(",", validLanguageList);
                foreach (var language in validLanguageList) {
                    if (_languageList.ContainsKey(language)) continue;
                    var languageDefinition = Languages[language];
                    _languageList.Add(language, languageDefinition.LanguageName);
                }

            }
            _countryList = new Dictionary<string, string>(Countries.Count);
            foreach (var item in Countries) { _countryList.Add(item.Key, item.Value.Country); }

            var cityListWithAlternates = new HashSet<string>();
            var cityList = new List<string>();
            foreach (var nameItems in CountryNames) {
                foreach (var item in nameItems.Value) {
                    if (!cityListWithAlternates.Contains(item.Name))
                        cityListWithAlternates.Add(item.Name);
                    if (!cityList.Contains(item.Name))
                        cityList.Add(item.Name);
                    if (!string.IsNullOrWhiteSpace(item.AlternateNames))
                        foreach (var name in item.AlternateNamesList) {
                            if (!cityListWithAlternates.Contains(name))
                                cityListWithAlternates.Add(name);
                        }
                }
            }
            _cityListWithAlternates = cityListWithAlternates.OrderBy(p=>p).ToList();
            _cityList = cityList.OrderBy(p=>p).ToList();
            Initialized = true;
        }

        public IDictionary<int, string> GetNames(string countryCode)
        {
            var nameItems = CountryNames[countryCode];
            if (nameItems == null) return new Dictionary<int, string>();
            var list = new Dictionary<int, string>(nameItems.Count);
            foreach (var name in nameItems) {
                list.Add(name.GeoNameId, name.Name);
            }
            return list;
        }

        public IDictionary<int, IList<string>> GetNamesWithAlternates(string countryCode)
        {
            var nameItems = CountryNames[countryCode];
            if (nameItems == null) return new Dictionary<int, IList<string>>();
            var list = new Dictionary<int, IList<string>>(nameItems.Count);
            foreach (var item in nameItems) {
                var items = new List<string> { item.Name };
                items.AddRange(item.AlternateNames.Split(','));
                list.Add(item.GeoNameId, items);
            }
            return list;
        }

        public IDictionary<string, string> GetCountries() { return _countryList; }
        public IList<string> GetCitiesWithAlternates() { return _cityListWithAlternates; }
        public IList<string> GetCities() { return _cityList; }
        public IDictionary<string, string> GetCities(string countryCode) { return CountryCities.ContainsKey(countryCode)? CountryCities[countryCode] : new Dictionary<string, string>(); }
        public IDictionary<string, string> GetLanguages() { return _languageList; }

        public IDictionary<string, string> GetLanguages(string countryCode)
        {
            var country = Countries[countryCode];
            var list = new Dictionary<string, string>();
            var languages = country.LanguagesList;
            foreach (var language in languages) {
                var languageName = _languageList[language];
                if (!list.ContainsKey(language))
                    list.Add(language, languageName);
            }
            return list;
        }

        public string GetValue(string lookupName, string key, string countryCode = "")
        {
            IDictionary<string, string> keyValues = null;
            var lookup = lookupName.ToLowerInvariant();
            switch (lookup) {
                case "language":
                    keyValues = GetLanguages();
                    break;
                case "country":
                    keyValues = GetCountries();
                    break;
                case "city":
                    keyValues = GetCities(countryCode);
                    break;
            }
            var value = (keyValues == null || String.IsNullOrWhiteSpace(key)) ? "" : (keyValues.ContainsKey(key)) ? keyValues[key] : "";
            return value;
        }

        public bool ContainsKey(string lookupName, string key, string countryCode = "")
        {
            IDictionary<string, string> keyValues = null;
            var lookup = lookupName.ToLowerInvariant();
            switch (lookup) {
                case "language":
                    keyValues = GetLanguages();
                    break;
                case "country":
                    keyValues = GetCountries();
                    break;
                case "city":
                    keyValues = GetCities(countryCode);
                    break;
            }
            var value = !(keyValues == null || String.IsNullOrWhiteSpace(key)) && keyValues.ContainsKey(key);
            return value;
        }

    }
}