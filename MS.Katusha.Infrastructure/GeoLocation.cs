using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Infrastructure
{
    public class GeoLocation
    {
        public IDictionary<string, GeoLanguage> Languages { get; private set; }
        public IDictionary<string, GeoCountry> Countries { get; private set; }
        public IDictionary<string, GeoName> Names { get; private set; }
        public IDictionary<string, GeoTimeZone> TimeZones { get; private set; }

        private IDictionary<string, IDictionary<string, string>> _countryCities { get; set; }
        private IDictionary<string, IList<GeoName>> _countryNames { get; set; }
        private IDictionary<string, string> _languageList;
        private IDictionary<string, string> _countryList;
        private IDictionary<string, string> _cityList;

        public bool Initialized;

        public GeoLocation()
        {
            Languages = new Dictionary<string, GeoLanguage>();
            Countries = new Dictionary<string, GeoCountry>();
            Names = new Dictionary<string, GeoName>();
            TimeZones = new Dictionary<string, GeoTimeZone>();
            Initialized = false;
        }

        public void Initialize()
        {
            _countryNames = new Dictionary<string, IList<GeoName>>(Countries.Count);
            _countryCities = new Dictionary<string, IDictionary<string, string>>(Countries.Count);
            foreach (var nameItem in Names) {
                var name = nameItem.Value;
                var key = name.CountryCode.ToLowerInvariant();
                IList<GeoName> geoNames;
                IDictionary<string, string> cityNames;
                if (!_countryNames.ContainsKey(key)) {
                    geoNames = new List<GeoName>();
                    _countryNames.Add(key, geoNames);
                    cityNames = new Dictionary<string, string>();
                    _countryCities.Add(key, cityNames);
                } else {
                    geoNames = _countryNames[key];
                    cityNames = _countryCities[key];
                }
                geoNames.Add(name);
                if(!cityNames.ContainsKey(name.GeoNameId.ToString(CultureInfo.InvariantCulture)))
                    cityNames.Add(name.GeoNameId.ToString(CultureInfo.InvariantCulture), name.Name);
            }
            var removeList = (from country in Countries where !_countryNames.ContainsKey(country.Key) select country.Key).ToList();
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
            var cityList = new Dictionary<string, string>();
            foreach (var nameItems in _countryNames) {
                foreach (var item in nameItems.Value) {
                    if (!cityList.ContainsKey(item.GeoNameId.ToString(CultureInfo.InvariantCulture)))
                        cityList.Add(item.GeoNameId.ToString(CultureInfo.InvariantCulture), item.Name);
                }
            }            
            _cityList = cityList; //.OrderBy(p=>p.Value.ToLowerInvariant());
            Initialized = true;
        }

        public IDictionary<string, string> GetCountries() { return _countryList; }

        public IDictionary<string, string> GetCities(string countryCode)
        {
            if (String.IsNullOrWhiteSpace(countryCode)) return _cityList;
            if (!_countryCities.ContainsKey(countryCode)) return new Dictionary<string, string>();
            return _countryCities[countryCode];
        }

        private IDictionary<string, string> GetCityList(string countryCode) { return _countryCities.ContainsKey(countryCode)? _countryCities[countryCode] : new Dictionary<string, string>(); }
        public IDictionary<string, string> GetLanguages() { return _languageList; }

        public string GetValue(string lookupName, string key, string countryCode = "")
        {
            var keyValues = GetLookup(lookupName, countryCode);
            var value = (keyValues == null || String.IsNullOrWhiteSpace(key)) ? "" : (keyValues.ContainsKey(key)) ? keyValues[key] : "";
            return value;
        }

        public IDictionary<string, string> GetLookup(string lookupName, string countryCode)
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
                    keyValues = GetCityList(countryCode);
                    break;
            }
            return keyValues ?? new Dictionary<string, string>();
        }
    }
}