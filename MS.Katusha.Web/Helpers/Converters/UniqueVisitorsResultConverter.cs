using System;
using AutoMapper;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Helpers.Converters
{
    public class UniqueVisitorsResultConverter : ITypeConverter<UniqueVisitorsResult, NewVisitModel>
    {
        private static UniqueVisitorsResultConverter _instance = null;

        private UniqueVisitorsResultConverter() { }

        public static UniqueVisitorsResultConverter GetInstance()
        { 
            if (_instance == null)
                _instance = new UniqueVisitorsResultConverter();
            return _instance;
        }

        public IProfileService ProfileService { get; set; }

        public NewVisitModel Convert(ResolutionContext context)
        {
            var data = context.SourceValue as UniqueVisitorsResult;
            if (data == null) throw new ArgumentNullException();
            var model = new NewVisitModel {
                Count = data.Count,
                LastVisitTime = data.LastVisitTime,
                Profile = Mapper.Map<ProfileModel>(ProfileService.GetProfile(data.ProfileId)),
                VisitorProfile = Mapper.Map<ProfileModel>(ProfileService.GetProfile(data.VisitorProfileId))
            };
            return model;
        }
    }
}