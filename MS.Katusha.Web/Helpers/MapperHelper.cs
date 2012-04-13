using System;
using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Services;
using MS.Katusha.Web.Models.Entities;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Helpers
{
    public class ConversationResultTypeConverter : ITypeConverter<ConversationResult, ConversationResultModel>
    {
        private static ConversationResultTypeConverter _instance = null;

        private ConversationResultTypeConverter() {}

        public static ConversationResultTypeConverter GetInstance() { 
            if (_instance == null) 
                _instance = new ConversationResultTypeConverter();
            return _instance;
        }

        public IProfileService ProfileService { get; set; }

        public ConversationResultModel Convert(ResolutionContext context)
        {
            var data = context.SourceValue as ConversationResult;
            if (data == null) throw new ArgumentNullException();
            var model = new ConversationResultModel {
                Count = data.Count,
                UnreadCount = data.UnreadCount,
                From = Mapper.Map<ProfileModel>(ProfileService.GetProfile(data.FromId)),
                To = Mapper.Map<ProfileModel>(ProfileService.GetProfile(data.ToId))
            };
            return model;
        }
    }
    
    public static class MapperHelper
    {

        public static void HandleMappings()
        {
            MS.Katusha.Services.Helpers.MapperHelper.HandleMappings();

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

            Mapper.CreateMap<SearchingFor, SearchingForModel>();
            Mapper.CreateMap<SearchingForModel, SearchingFor>();

            Mapper.CreateMap<SearchCriteria, SearchCriteriaModel>();
            Mapper.CreateMap<SearchCriteriaModel, SearchCriteria>();

            Mapper.CreateMap<SearchResult, SearchResultModel>();
            Mapper.CreateMap<SearchResultModel, SearchResult>();

            Mapper.CreateMap<Visit, VisitModel>();
            Mapper.CreateMap<VisitModel, Visit>();

            Mapper.CreateMap<Profile, ProfileModel>();
            Mapper.CreateMap<ProfileModel, Profile>();

        }
    }
}