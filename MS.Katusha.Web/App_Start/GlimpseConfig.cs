using System.Web;
using System.Web.Mvc;
using MS.Katusha.DependencyManagement;

namespace MS.Katusha.Web
{
    public static class GlimpseConfig
    {
        public static void RegisterGlimpse(HttpApplicationState application)
        {
            var store = DependencyRegistrar.RegisterGlimpse();
            if(store != null)
                application["MyDocStore"] = store;
        }
    }
}
