using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.Mvc;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Helpers.Binders
{
    public class FacebookProfileModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var facebookProfileModel = base.BindModel(controllerContext, bindingContext) as FacebookProfileModel;
            if(facebookProfileModel == null) return null;
            var resourceService = DependencyResolver.Current.GetService<IResourceService>();
            var formValues = controllerContext.HttpContext.Request.Form;

            var countryCode = formValues["Country"];
            facebookProfileModel.Location = new LocationModel();
            if (countryCode != null && countryCode.Length == 2 && resourceService.ContainsKey("Country", countryCode)) {
                facebookProfileModel.Location.CountryCode = countryCode;
                facebookProfileModel.Location.CountryName = resourceService.GetLookupText("Country", countryCode);
                int code;
                if (int.TryParse(formValues["CityKey"], out code) && resourceService.CountryHasCity(countryCode, code)) {
                    facebookProfileModel.Location.CityCode = code;
                    facebookProfileModel.Location.CityName = resourceService.GetLookupText("City", code.ToString(CultureInfo.InvariantCulture), countryCode);
                }
            }
            return facebookProfileModel;
        }
    }
}