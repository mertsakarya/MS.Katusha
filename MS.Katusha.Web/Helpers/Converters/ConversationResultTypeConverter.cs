using System;
using AutoMapper;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Helpers.Converters
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
                Subject = data.Subject,
                From = Mapper.Map<ProfileModel>(ProfileService.GetProfile(data.FromId)),
                To = Mapper.Map<ProfileModel>(ProfileService.GetProfile(data.ToId))
            };
            return model;
        }
    }
}