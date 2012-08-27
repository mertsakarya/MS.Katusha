using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Indexes;
using Raven.Client.Linq;

namespace MS.Katusha.Repositories.RavenDB
{
    public class CountryCityCountRepositoryRavenDB : ICountryCityCountRepositoryRavenDB
    {
        private readonly IKatushaRavenStore _documentStore;

        public CountryCityCountRepositoryRavenDB(IKatushaRavenStore documentStore) { _documentStore = documentStore; }

        public IDictionary<string, string> GetSearchableCities(Sex gender, string countryCode)
        {
            using (var session = _documentStore.OpenSession()) {
                try {
                    var query = Queryable.OrderByDescending(session.Query<CountryCityCountResult, CountryCityCountIndex>().Where(p => p.CountryCode == countryCode && p.Gender == (byte) gender), p => p.Count);
                    return query.ToDictionary(item => item.CityCode.ToString(CultureInfo.InvariantCulture), item => item.CityName);
                } catch(InvalidOperationException) {
                    return new Dictionary<string, string>();
                }
            }
        }

        public IDictionary<string, string> GetSearchableCountries(Sex gender)
        {
            using (var session = _documentStore.OpenSession()) {
                try {
                    var query = (from item in session.Query<CountryCityCountResult, CountryCityCountIndex>().Where(p => p.Gender == (byte)gender).ToList()
                                 group item by new {item.CountryCode, item.CountryName}
                                     into g
                                     let o = new {Sum = g.Sum(x => x.Count), g.Key.CountryCode, g.Key.CountryName}
                                     orderby o.Sum descending
                                     select new {g.Key.CountryCode, g.Key.CountryName});
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    foreach (var item in query)
                        if(item != null && !String.IsNullOrWhiteSpace(item.CountryCode))
                            dictionary.Add(item.CountryCode, item.CountryName);
                    return dictionary;
                } catch(InvalidOperationException) {
                    return new Dictionary<string, string>();
                }
            }


            
        }
    }
}