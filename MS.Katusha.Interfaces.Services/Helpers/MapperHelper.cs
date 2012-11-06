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
            MS.Katusha.Interfaces.Repositories.Helpers.MapperHelper.HandleMappings();
            Mapper.CreateMap<MS.Katusha.Domain.Entities.Conversation, MS.Katusha.Domain.Raven.Entities.Conversation>()
                .ForMember(dest => dest.FromName, opt => opt.MapFrom(src => src.From.Name))
                .ForMember(dest => dest.ToName, opt => opt.MapFrom(src => src.To.Name))
                .ForMember(dest => dest.FromGuid, opt => opt.MapFrom(src => src.From.Guid))
                .ForMember(dest => dest.ToGuid, opt => opt.MapFrom(src => src.To.Guid))
                .ForMember(dest => dest.FromPhotoGuid, opt => opt.MapFrom(src => src.From.ProfilePhotoGuid))
                .ForMember(dest => dest.ToPhotoGuid, opt => opt.MapFrom(src => src.To.ProfilePhotoGuid))
                ;
            Mapper.CreateMap<MS.Katusha.Domain.Raven.Entities.Conversation, MS.Katusha.Domain.Entities.Conversation>();

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

}