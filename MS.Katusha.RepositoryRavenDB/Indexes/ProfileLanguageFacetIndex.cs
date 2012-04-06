using System.Linq;
using MS.Katusha.Domain.Entities;
using Raven.Client.Indexes;


namespace MS.Katusha.Repositories.RavenDB.Indexes
{
    public class ProfileLanguageFacetIndex : AbstractIndexCreationTask<Profile>
    {
        public ProfileLanguageFacetIndex()
        {
            Map = profiles => from p in profiles
                              from l in p.LanguagesSpoken
                              select new {
                                  p.From, p.City, p.Gender, p.BodyBuild, p.HairColor, p.EyeColor, p.Smokes, p.Alcohol, p.Religion, p.DickSize, p.DickThickness, p.BreastSize, p.BirthYear, p.Height,

                                  l.Language
                              };
        }
    }
}
