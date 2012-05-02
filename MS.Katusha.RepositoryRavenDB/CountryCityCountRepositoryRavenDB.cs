using System;
using System.Collections.Generic;
using System.Linq;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace MS.Katusha.Repositories.RavenDB
{
    public class CountryCityCountRepositoryRavenDB : ICountryCityCountRepositoryRavenDB
    {
        private readonly IDocumentStore _documentStore;

        public CountryCityCountRepositoryRavenDB(IDocumentStore documentStore) { _documentStore = documentStore; }

        public IList<string> GetSearchableCities(Sex gender, string countryCode)
        {
            using (var session = _documentStore.OpenSession()) {
                try {
                    return Queryable.OrderByDescending(session.Query<CountryCityCountResult, CountryCityCountIndex>().Where(p => p.Country == countryCode && p.Gender == (byte)gender), p => p.Count).Select(item => item.City).ToList();
                } catch(InvalidOperationException) {
                    return new List<string>();
                }
            }
        }

        public IList<string> GetSearchableCountries(Sex gender)
        {
            using (var session = _documentStore.OpenSession()) {
                return (from item in session.Query<CountryCityCountResult, CountryCityCountIndex>().Where(p=> p.Gender == (byte)gender).ToList()
                        group item by item.Country
                        into g
                        let sum = g.Sum(x => x.Count)
                        orderby sum descending
                        select g.Key).ToList();
            }
        }
    }
}