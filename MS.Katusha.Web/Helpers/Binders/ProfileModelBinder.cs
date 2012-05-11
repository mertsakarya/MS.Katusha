using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Helpers.Binders
{
    public class ProfileModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var profileModel = base.BindModel(controllerContext, bindingContext) as ProfileModel;
            if(profileModel == null) return null;
            var resourceService = DependencyResolver.Current.GetService<IResourceService>();
            var formValues = controllerContext.HttpContext.Request.Form;

            var countryCode = formValues["Country"];
            profileModel.Location = new LocationModel();
            if (countryCode != null && countryCode.Length == 2 && resourceService.ContainsKey("Country", countryCode)) {
                profileModel.Location.CountryCode = countryCode;
                profileModel.Location.CountryName = resourceService.GetLookupText("Country", countryCode);
                int code;
                if (int.TryParse(formValues["CityKey"], out code) && resourceService.CountryHasCity(countryCode, code)) {
                    profileModel.Location.CityCode = code;
                    profileModel.Location.CityName = resourceService.GetLookupText("City", code.ToString(CultureInfo.InvariantCulture), countryCode);
                }
            }
            
            var languagesSpokenModelList = new List<LanguagesSpokenModel>();
            var formValue = formValues["LanguagesSpokenModelSelection[]"];
            if (formValue != null)
                foreach (var item in formValue.Split(','))
                    if (resourceService.ContainsKey("Language", item))
                        languagesSpokenModelList.Add(new LanguagesSpokenModel { Language = item });
            profileModel.LanguagesSpoken = languagesSpokenModelList;

            var countriesToVisitModelList = new List<CountriesToVisitModel>();
            formValue = formValues["CountriesToVisitModelSelection[]"];
            if (formValue != null)
                foreach (var item in formValue.Split(','))
                    if (resourceService.ContainsKey("Country", item))
                        countriesToVisitModelList.Add(new CountriesToVisitModel { Country = item });
            profileModel.CountriesToVisit = countriesToVisitModelList;

            var searchingForModelList = new List<SearchingForModel>();
            formValue = formValues["SearchingForModelSelection[]"];
            if (formValue != null) {
                foreach (var item in formValue.Split(',')) {
                    LookingFor lookingFor;
                    if (Enum.TryParse(item, true, out lookingFor)) {
                        searchingForModelList.Add(new SearchingForModel {Search = lookingFor});
                    }
                }
            }
            profileModel.Searches = searchingForModelList;

            return profileModel;
        }
    }
}