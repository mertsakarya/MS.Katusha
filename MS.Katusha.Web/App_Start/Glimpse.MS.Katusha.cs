using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Glimpse.Core.Extensibility;

namespace MS.Katusha.Web.App_Start
{
    [GlimpsePlugin(ShouldSetupInInit = true)]
    public class Profiler : IGlimpsePlugin, IProvideGlimpseHelp
    {
        public object GetData(HttpContextBase context)
        {
            var data = new List<object[]> {
                new object[] {"Key", "Value"},
                new [] {"Action", context.Items["ActionName"]},
                new [] {"Controller", context.Items["ControllerName"]},
            };
            var viewData = context.Items["ViewData"] as ViewDataDictionary;
            if (viewData != null) {
                var model = viewData.Model;
                data.Add(new [] {"Model", model});
                foreach (var item in viewData) {
                    data.Add(new [] {item.Key, item.Value});
                }
            }
            return data;
        }

        public void SetupInit()
        {
        }

        public string Name
        {
            get { return "MS.Katusha"; }
        }

        public string HelpUrl
        {
            get { return "http://www.mskatusha.com/"; }
        }
    }
}