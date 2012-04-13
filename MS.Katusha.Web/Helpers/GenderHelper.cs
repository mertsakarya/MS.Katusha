using AutoMapper;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Exceptions.Services;
using MS.Katusha.Web.Models.Entities;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Helpers
{
    public static class GenderHelper
    {
        private const string Men = "Men";
        private const string Girls = "Girls";

        public static string GetControllerName(Profile profile)
        {
            if (profile.Gender == (byte)Sex.Male)
                return Men;
            if (profile.Gender == (byte)Sex.Female)
                return Girls;
            throw new KatushaGenderNotExistsException(profile);
        }

        public static string GetControllerName(ProfileModel profileModel)
        {
            if (profileModel.Gender == Sex.Male)
                return Men;
            if (profileModel.Gender == Sex.Female)
                return Girls;
            throw new KatushaGenderNotExistsException(Mapper.Map<Profile>(profileModel));
        }
    }
}