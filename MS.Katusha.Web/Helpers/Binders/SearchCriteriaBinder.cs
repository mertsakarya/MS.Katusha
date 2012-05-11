using System.Globalization;
using System.Web.Mvc;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Helpers.Binders
{
    public class SearchCriteriaBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var searchCriteriaModel = base.BindModel(controllerContext, bindingContext) as BaseSearchCriteriaModel;

            if (searchCriteriaModel != null) {
                var resourceService = DependencyResolver.Current.GetService<IResourceService>();
                var formValues = controllerContext.HttpContext.Request.QueryString;
                var countryCode = formValues["CountryCode"];
                searchCriteriaModel.Location = new LocationModel();
                if (countryCode != null && countryCode.Length == 2 && resourceService.ContainsKey("Country", countryCode)) {
                    searchCriteriaModel.Location.CountryCode = countryCode;
                    searchCriteriaModel.Location.CountryName = resourceService.GetLookupText("Country", countryCode);
                    int code;
                    if (int.TryParse(formValues["CityCode"], out code) && resourceService.CountryHasCity(countryCode, code)) {
                        searchCriteriaModel.Location.CityCode = code;
                        searchCriteriaModel.Location.CityName = resourceService.GetLookupText("City", code.ToString(CultureInfo.InvariantCulture), countryCode);
                    }
                }
            }
            return searchCriteriaModel;
        }
    }
}