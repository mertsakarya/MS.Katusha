using System.Linq;
using MS.Katusha.Domain.Entities;
using Raven.Client.Indexes;


namespace MS.Katusha.Repositories.RavenDB.Indexes
{
    public class StateFacetsIndex : AbstractIndexCreationTask<State>
    {
        public StateFacetsIndex()
        {
            Map = states => from p in states
                              select new {
                                  From = p.CountryCode, City = p.CityCode, p.Gender, p.BodyBuild, p.HairColor, p.EyeColor, p.Height
                              };
        }
    }
}
