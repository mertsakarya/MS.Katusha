using System.Collections.Generic;

namespace MS.Katusha.Interfaces.Services
{
    public interface ILocationService {
        IDictionary<string, string> GetCountries();
        IDictionary<int, string> GetCities(string countryCode);
        IDictionary<int, IList<string>> GetCitiesWithAlternates(string countryCode);
        IDictionary<string, string> GetLanguages();
        IDictionary<string, string> GetLanguages(string countryCode);
        IList<string> GetCities();
        IList<string> GetCitiesWithAlternates();
    }
}