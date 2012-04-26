using System.Collections.Generic;
using MS.Katusha.Infrastructure;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{

    public class LocationService : ILocationService
    {
        private readonly IResourceManager _resourceManager;
        private readonly IKatushaGlobalCacheContext _katushaGlobalCache;
        private readonly GeoLocation _geoLocation;

        public LocationService(IResourceManager resourceManager, IKatushaGlobalCacheContext globalCacheContext)
        {
            _resourceManager = resourceManager;
            _geoLocation = _resourceManager.GeoLocation;
            _katushaGlobalCache = globalCacheContext;
        }

        public IDictionary<string, string> GetCountries() { return _geoLocation.GetCountries(); }
        public IDictionary<string, string> GetLanguages() { return _geoLocation.GetLanguages(); }
        public IList<string> GetCities() { return _geoLocation.GetCities(); }
        public IList<string> GetCitiesWithAlternates() { return _geoLocation.GetCitiesWithAlternates(); }
        public IDictionary<int, string> GetCities(string countryCode) { return _geoLocation.GetNames(countryCode.ToLowerInvariant()); }
        public IDictionary<int, IList<string>> GetCitiesWithAlternates(string countryCode) { return _geoLocation.GetNamesWithAlternates(countryCode.ToLowerInvariant()); }
        public IDictionary<string, string> GetLanguages(string countryCode) { return _geoLocation.GetLanguages(countryCode.ToLowerInvariant()); }
    }
}
