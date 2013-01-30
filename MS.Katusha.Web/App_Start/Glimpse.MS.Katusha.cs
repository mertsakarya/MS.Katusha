using System.Collections.Generic;
using System.Web;
using Glimpse.Core.Extensibility;

namespace MS.Katusha.Web.App_Start
{
    [GlimpsePlugin(ShouldSetupInInit = true)]
    public class Profiler : IGlimpsePlugin, IProvideGlimpseHelp
    {
        public object GetData(HttpContextBase context)
        {
            dynamic viewBag = context.Items["ViewBag"];
            var data = new List<object[]> {
                new object[] {"Key", "Value"},
                new object[] {"Profile", viewBag.KatushaProfile},
                new object[] {"User", viewBag.KatushaUser},
                new object[] {"State", viewBag.KatushaState},
                new object[] {"OnlineProfiles", viewBag.KatushaOnlineProfiles},
                new object[] {"NewProfiles", viewBag.KatushaNewProfiles},
                new object[] {"SearchResult", viewBag.KatushaSearchResult},
                new object[] {"GoogleAnalytics", viewBag.GoogleAnalytics},
                new object[] {"Title", viewBag.Title},
                new object[] {"SameProfile", viewBag.SameProfile},
                new object[] {"HasLayout", viewBag.HasLayout},
                new object[] {"FormAction", viewBag.FormAction},
                new object[] {"Message", viewBag.Message},
            };
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