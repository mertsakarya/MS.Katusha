using System.Collections.Generic;

namespace MS.Katusha.Interfaces.Services
{
    public interface ILocationService {
        IDictionary<string, string> GetCountries();
        IList<string> GetCities(string countryCode);
        IList<IList<string>> GetCitiesWithAlternates(string countryCode);
        IDictionary<string, string> GetLanguages();
        IDictionary<string, string> GetLanguages(string countryCode);
    }
}