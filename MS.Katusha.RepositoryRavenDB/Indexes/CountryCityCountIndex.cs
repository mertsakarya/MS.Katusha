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
            Map = docs => from doc in docs select new CountryCityCountResult { Gender = doc.Gender, Country = doc.From, City = doc.City, Count = 1 };
            Reduce = results => from result in results
                                group result by new {result.Gender, result.Country, result.City}
                                into g
                                select new CountryCityCountResult { Gender = g.Key.Gender, Country = g.Key.Country, City = g.Key.City, Count = g.Sum(m => m.Count) };
        }
    }
}