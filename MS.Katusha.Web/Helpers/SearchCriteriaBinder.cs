using System.Web.Mvc;
using MS.Katusha.Enumerations;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Helpers
{
    public class SearchCriteriaBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var searchCriteriaModel = base.BindModel(controllerContext, bindingContext) as SearchCriteriaModel;
            return searchCriteriaModel;
        }
    }
}