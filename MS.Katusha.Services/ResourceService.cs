using System.Collections.Generic;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class ResourceService : IResourceService
    {
        private readonly ICountryCityCountRepositoryRavenDB _countryCityCountRepository;
        private readonly ResourceManager _resourceManager;

        public ResourceService(ICountryCityCountRepositoryRavenDB countryCityCountRepository)
        {
            _countryCityCountRepository = countryCityCountRepository;
            _resourceManager = ResourceManager.GetInstance();
        }

        public string ConfigurationValue(string propertyName) { return _resourceManager.ConfigurationValue(propertyName); }
        public string ConfigurationValue(string propertyName, string key, bool mustFind = false) { return _resourceManager.ConfigurationValue(propertyName, key, mustFind); }
        public string ResourceValue(string resourceName, string language = "") { return _resourceManager.ResourceValue(resourceName, language); }
        public string ResourceValue(string propertyName, string key, bool mustFind, string language) { return _resourceManager.ResourceValue(propertyName, key, mustFind, language); }
        public IDictionary<string, string> GetLookup(string lookupName, string countryCode = "") { return _resourceManager.GetLookup(lookupName, countryCode); }
        public string GetLookupText(string lookupName, string key, string countryCode = "") { return _resourceManager.GetLookupText(lookupName, key, countryCode); }
        public string GetLookupEnumKey(string lookupName, byte value, string language = "") { return _resourceManager.GetLookupEnumKey(lookupName, value, language); }
        public bool ContainsKey(string lookupName, string key, string countryCode = "") { return _resourceManager.ContainsKey(lookupName, key, countryCode); }
        public IDictionary<string, string> GetCountries() { return _resourceManager.GetCountries(); }
        public IDictionary<string, string> GetLanguages() { return _resourceManager.GetLanguages(); }
        public IList<string> GetCities(string countryCode) { return _resourceManager.GetCities(countryCode); }

        public IDictionary<string, string> GetSearchableCountries(Sex gender)
        {
            var countryCodeList = _countryCityCountRepository.GetSearchableCountries(gender);
            var result = new Dictionary<string, string>(countryCodeList.Count);
            if (countryCodeList.Count > 0) {
                var countries = GetCountries();
                foreach (var countryCode in countryCodeList) {
                    if(countries.ContainsKey(countryCode))
                        result.Add(countryCode, countries[countryCode]);
                }
            }
            return result;
        }

        public IList<string> GetSearchableCities(Sex gender, string countryCode) { return _countryCityCountRepository.GetSearchableCities(gender, countryCode); }
    }

}
