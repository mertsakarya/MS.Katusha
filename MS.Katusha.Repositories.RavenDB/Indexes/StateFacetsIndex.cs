using System;
using System.Linq;
using MS.Katusha.Domain.Entities;
using Raven.Client.Indexes;


namespace MS.Katusha.Repositories.RavenDB.Indexes
{
    public class StateFacetsIndex : AbstractIndexCreationTask<State>
    {
        public StateFacetsIndex()
        {
            Map = states => from p in states // where p.LastOnline > DateTime.Now - new TimeSpan(0,0,5)
                select new {
                    p.CountryCode, 
                    p.CityCode, 
                    p.Gender, 
                    p.BodyBuild, 
                    p.HairColor, 
                    p.EyeColor, 
                    p.Height,
                    p.BirthYear
                };
        }
    }
}
