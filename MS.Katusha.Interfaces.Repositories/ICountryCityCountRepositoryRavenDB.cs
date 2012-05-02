using System.Collections.Generic;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface ICountryCityCountRepositoryRavenDB {
        IList<string> GetSearchableCities(Sex gender, string countryCode);
        IList<string> GetSearchableCountries(Sex gender);
    }
}