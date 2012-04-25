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
        private GeoLocation _geoLocation;

        public LocationService(IResourceManager resourceManager, IKatushaGlobalCacheContext globalCacheContext)
        {
            _resourceManager = resourceManager;
            _geoLocation = _resourceManager.GeoLocation;
            _katushaGlobalCache = globalCacheContext;
        }

        public IDictionary<string, string> GetCountries() { return _geoLocation.GetCountryNames(); }
        public IList<string> GetCities(string countryCode) { return _geoLocation.GetNames(countryCode); }
        public IList<IList<string>> GetCitiesWithAlternates(string countryCode) { return _geoLocation.GetNamesWithAlternates(countryCode); }
        public IDictionary<string, string> GetLanguages() { return _geoLocation.GetLanguages(); }
        public IDictionary<string, string> GetLanguages(string countryCode) { return _geoLocation.GetLanguages(countryCode); }
    }
}
