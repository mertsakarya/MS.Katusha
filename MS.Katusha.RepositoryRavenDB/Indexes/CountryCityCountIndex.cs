using System;
using System.Collections.Generic;
using System.Linq;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using Raven.Client.Indexes;

namespace MS.Katusha.Repositories.RavenDB.Indexes
{
    public class CountryCityCountIndex : AbstractIndexCreationTask<Profile, CountryCityCountResult>
    {
        public CountryCityCountIndex()
        {
            Map = docs => from doc in docs select new CountryCityCountResult {
                                                                                 Gender = doc.Gender, 
                                                                                 CountryCode = doc.Location.CountryCode, 
                                                                                 CityCode = doc.Location.CityCode, 
                                                                                 CityName = doc.Location.CityName, 
                                                                                 CountryName = doc.Location.CountryName, 
                                                                                 Count = 1
                                                                             };
            Reduce = results => from result in results
                                group result by new {result.Gender, result.CountryCode, result.CityCode, result.CityName, result.CountryName}
                                into g
                                select new CountryCityCountResult {
                                                                      Gender = g.Key.Gender, 
                                                                      CountryCode = g.Key.CountryCode, 
                                                                      CityCode = g.Key.CityCode,
                                                                      CityName = g.Key.CityName,
                                                                      CountryName = g.Key.CountryName,
                                                                      Count = g.Sum(m => m.Count)
                                                                  };
        }
    }
}