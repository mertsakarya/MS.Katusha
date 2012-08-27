using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Service;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Interfaces.Services.Helpers
{
    public static class MapperHelper
    {
        public static void HandleMappings()
        {

            Mapper.CreateMap<MS.Katusha.Domain.Entities.Conversation, MS.Katusha.Domain.Raven.Entities.Conversation>()
                .ForMember(dest => dest.FromName, opt => opt.MapFrom(src => src.From.Name))
                .ForMember(dest => dest.ToName, opt => opt.MapFrom(src => src.To.Name))
                .ForMember(dest => dest.FromGuid, opt => opt.MapFrom(src => src.From.Guid))
                .ForMember(dest => dest.ToGuid, opt => opt.MapFrom(src => src.To.Guid))
                .ForMember(dest => dest.FromPhotoGuid, opt => opt.MapFrom(src => src.From.ProfilePhotoGuid))
                .ForMember(dest => dest.ToPhotoGuid, opt => opt.MapFrom(src => src.To.ProfilePhotoGuid))
                ;
            Mapper.CreateMap<MS.Katusha.Domain.Raven.Entities.Conversation, MS.Katusha.Domain.Entities.Conversation>();
            Mapper.CreateMap<Profile, State>().ConvertUsing<ProfileStateTypeConverter>();

            Mapper.CreateMap<Profile, ApiProfile>();
            Mapper.CreateMap<Photo, ApiPhoto>();
            Mapper.CreateMap<PhotoBackup, ApiPhotoBackup>();
            Mapper.CreateMap<Location, ApiLocation>();
            Mapper.CreateMap<User, ApiUser>();
            Mapper.CreateMap<User, ApiAdminUser>();
            Mapper.CreateMap<MS.Katusha.Domain.Raven.Entities.Conversation, ApiConversation>();

            Mapper.CreateMap<ApiProfile, Profile>();
            Mapper.CreateMap<ApiPhoto, Photo>();
            Mapper.CreateMap<ApiPhotoBackup, PhotoBackup>();
            Mapper.CreateMap<ApiLocation, Location>();
            Mapper.CreateMap<ApiUser, User>();
            Mapper.CreateMap<ApiAdminUser, User>();
            Mapper.CreateMap<ApiConversation, MS.Katusha.Domain.Raven.Entities.Conversation>();
        }
    }

    public class ProfileStateTypeConverter : ITypeConverter<Profile, State>
    {
        public State Convert(ResolutionContext context)
        {
            var data = context.SourceValue as Domain.Entities.Profile;
            if (data == null) throw new ArgumentNullException();
            var model = new State {
                BirthYear = data.BirthYear, BodyBuild = data.BodyBuild, CityCode = data.Location.CityCode, EyeColor = data.EyeColor, CountryCode = data.Location.CountryCode, HairColor = data.HairColor, Height = data.Height,
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