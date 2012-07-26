using System.Text;
using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Web.Helpers.Converters;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Helpers
{
    public static class MapperHelper
    {

        public static void HandleMappings()
        {
            Services.Helpers.MapperHelper.HandleMappings();

            Mapper.CreateMap<Profile, ProfileModel>();
            Mapper.CreateMap<ProfileModel, Profile>()
                .ForMember(dest => dest.BreastSize, opt => opt.MapFrom(src => (byte?) src.BreastSize))
                .ForMember(dest => dest.DickSize, opt => opt.MapFrom(src => (byte?) src.DickSize))
                .ForMember(dest => dest.DickThickness, opt => opt.MapFrom(src => (byte?) src.DickThickness));


            Mapper.CreateMap<ConversationModel, Conversation>();
            Mapper.CreateMap<Conversation, ConversationModel>();

            Mapper.CreateMap<ConversationResultModel, ConversationResult>()
                .ForMember(dest => dest.FromId, opt => opt.MapFrom(src => src.From.Id))
                .ForMember(dest => dest.ToId, opt => opt.MapFrom(src => src.To.Id));
            Mapper.CreateMap<ConversationResult, ConversationResultModel>().ConvertUsing(ConversationResultTypeConverter.GetInstance());

            Mapper.CreateMap<CountriesToVisit, CountriesToVisitModel>();
            Mapper.CreateMap<CountriesToVisitModel, CountriesToVisit>();

            Mapper.CreateMap<LanguagesSpoken, LanguagesSpokenModel>();
            Mapper.CreateMap<LanguagesSpokenModel, LanguagesSpoken>();

            Mapper.CreateMap<Photo, PhotoModel>();
            Mapper.CreateMap<PhotoModel, Photo>();

            Mapper.CreateMap<Location, LocationModel>();
            Mapper.CreateMap<LocationModel, Location>();

            Mapper.CreateMap<SearchingFor, SearchingForModel>();
            Mapper.CreateMap<SearchingForModel, SearchingFor>();

            Mapper.CreateMap<SearchStateCriteria, SearchStateCriteriaModel>();
            Mapper.CreateMap<SearchStateCriteriaModel, SearchStateCriteria>();

            Mapper.CreateMap<SearchProfileCriteria, SearchProfileCriteriaModel>();
            Mapper.CreateMap<SearchProfileCriteriaModel, SearchProfileCriteria>();

            Mapper.CreateMap<SearchResult, SearchStateResultModel>();
            Mapper.CreateMap<SearchStateResultModel, SearchResult>();

            Mapper.CreateMap<SearchResult, SearchProfileResultModel>();
            Mapper.CreateMap<SearchProfileResultModel, SearchResult>();

            Mapper.CreateMap<Visit, VisitModel>();
            Mapper.CreateMap<VisitModel, Visit>();

            Mapper.CreateMap<Profile, ProfileModel>();
            Mapper.CreateMap<ProfileModel, Profile>();
            Mapper.CreateMap<FacebookProfileModel, Profile>();

            Mapper.CreateMap<UniqueVisitorsResult, NewVisitModel>().ConvertUsing(UniqueVisitorsResultConverter.GetInstance());

        }
    }
}