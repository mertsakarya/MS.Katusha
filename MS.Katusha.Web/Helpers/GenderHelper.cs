using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using MS.Katusha.Enumerations;
using MS.Katusha.Exceptions.Services;
using MS.Katusha.Web.Models.Entities;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Helpers
{
    public static class GenderHelper
    {
        private const string Boys = "Boys";
        private const string Girls = "Girls";

        public static string GetControllerName(Profile profile)
        {
            if (profile.Gender == (byte)Sex.Male)
                return Boys;
            if (profile.Gender == (byte)Sex.Female)
                return Girls;
            throw new KatushaGenderNotExistsException(profile);
        }

        public static string GetControllerName(ProfileModel profileModel)
        {
            if (profileModel.Gender == (byte)Sex.Male)
                return Boys;
            if (profileModel.Gender == (byte)Sex.Female)
                return Girls;
            throw new KatushaGenderNotExistsException(Mapper.Map<Profile>(profileModel));
        }
    }
}