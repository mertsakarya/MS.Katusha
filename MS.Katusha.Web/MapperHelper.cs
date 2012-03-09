using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web
{
    public static class MapperHelper
    {
        public static void HandleMappings()
        {
            Mapper.CreateMap<Girl, GirlModel>();
            Mapper.CreateMap<GirlModel, Girl>();
        }
    }
}