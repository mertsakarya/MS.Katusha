using System.Linq;
using MS.Katusha.Domain.Entities;
using Raven.Client.Indexes;

namespace MS.Katusha.Repositories.RavenDB.Indexes
{
    public class ProfileSearchFacetIndex : AbstractIndexCreationTask<Profile>
    {
        public ProfileSearchFacetIndex()
        {
            Map = profiles => from p in profiles
                              from s in p.Searches
                              select new {
                                    p.From, p.City, p.Gender, p.BodyBuild, p.HairColor, p.EyeColor, p.Smokes, p.Alcohol, p.Religion, p.DickSize, p.DickThickness, p.BreastSize, p.BirthYear, p.Height,
                                    s.Search
                                         };
        }
    }
}