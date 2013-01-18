using System.Linq;
using MS.Katusha.Domain.Entities;
using Raven.Client.Indexes;


namespace MS.Katusha.Repositories.RavenDB.Indexes
{
    public class ProfileFacetsIndex : AbstractIndexCreationTask<Profile>
    {
        public ProfileFacetsIndex()
        {
            Map = profiles => from p in profiles
                              select new {
                                  p.Location.CountryCode, 
                                  p.Location.CityCode,
                                  p.Gender, 
                                  p.BodyBuild, 
                                  p.HairColor, 
                                  p.EyeColor, 
                                  p.Smokes, 
                                  p.Alcohol, 
                                  p.Religion, 
                                  p.DickSize, 
                                  p.DickThickness, 
                                  p.BreastSize, 
                                  p.BirthYear, 
                                  p.Height
                              };
        }
    }
}
