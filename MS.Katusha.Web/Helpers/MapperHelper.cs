using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Helpers
{
    public static class MapperHelper
    {
        public static void HandleMappings()
        {
            Mapper.CreateMap<Girl, GirlModel>();
            Mapper.CreateMap<GirlModel, Girl>();
            Mapper.CreateMap<Boy, BoyModel>();
            Mapper.CreateMap<BoyModel, Boy>();
        }
    }
}