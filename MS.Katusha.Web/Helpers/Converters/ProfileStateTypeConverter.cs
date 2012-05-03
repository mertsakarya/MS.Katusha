using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Web.Helpers.Converters
{
    public class ProfileStateTypeConverter : ITypeConverter<Domain.Entities.Profile, State>
    {
        public State Convert(ResolutionContext context)
        {
            var data = context.SourceValue as Domain.Entities.Profile;
            if (data == null) throw new ArgumentNullException();
            var model = new State {
                Birthyear = data.BirthYear, BodyBuild = data.BodyBuild, City = data.City, EyeColor = data.EyeColor, From = data.From, HairColor = data.HairColor, Height = data.Height,
                Gender = data.Gender, ProfileId = data.Id, Id = data.Id, LastOnline = DateTime.Now, HasPhoto = (data.Photos.Count > 0)
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