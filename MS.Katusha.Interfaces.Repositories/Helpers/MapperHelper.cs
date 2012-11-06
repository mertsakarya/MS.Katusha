using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using MS.Katusha.Domain.Entities;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Interfaces.Repositories.Helpers
{
    public static class MapperHelper
    {
        public static void HandleMappings()
        {
            Mapper.CreateMap<Profile, State>().ConvertUsing<ProfileStateTypeConverter>();
        }
    }

    public class ProfileStateTypeConverter : ITypeConverter<Profile, State>
    {
        public State Convert(ResolutionContext context)
        {
            var data = context.SourceValue as Profile;
            if (data == null) throw new ArgumentNullException();
            var model = new State {
                BirthYear = data.BirthYear, BodyBuild = data.BodyBuild, CityCode = data.Location.CityCode, EyeColor = data.EyeColor, CountryCode = data.Location.CountryCode, HairColor = data.HairColor, Height = data.Height,
                Gender = data.Gender, ProfileId = data.Id, HasPhoto = (data.Photos.Count > 0),
                Name = data.Name, PhotoGuid = data.ProfilePhotoGuid, ProfileGuid = data.Guid
            };

            var countriesToVisit = data.CountriesToVisit;
            if (countriesToVisit.Count > 0) {
                var list = new List<string>(countriesToVisit.Count);
                list.AddRange(countriesToVisit.Select(countryToVisit => countryToVisit.Country));
                model.CountriesToVisit = String.Join(",", list);
            } else model.CountriesToVisit = "";

            var searches = data.Searches;
            if (searches.Count > 0) {
                var list = new List<string>(searches.Count);
                list.AddRange(searches.Select(search => search.Search.ToString(CultureInfo.InvariantCulture)));
                model.Searches = String.Join(",", list);
            } else model.Searches = "";
            return model;
        }
    }

}
