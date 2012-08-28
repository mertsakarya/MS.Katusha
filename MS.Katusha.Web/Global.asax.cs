using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MS.Katusha.Repositories.DB.Context;
using MS.Katusha.Web.Helpers;

namespace MS.Katusha.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Context.Application.Lock();
            try {
                Database.SetInitializer(new KatushaContextInitializer());
                ModelMetadataProviders.Current = new KatushaMetadataProvider();
                DependencyConfig.RegisterDependencies();
                MapperHelper.HandleMappings();

                AreaRegistration.RegisterAllAreas();

                ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
                ValueProviderFactories.Factories.Add(new KatushaJsonValueProviderFactory());

                WebApiConfig.Register(GlobalConfiguration.Configuration);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleTable.EnableOptimizations = false;
                BundleConfig.RegisterBundles(BundleTable.Bundles);
                AuthConfig.RegisterAuth();
                GlimpseConfig.RegisterGlimpse(Application);
            } finally {
                Context.Application.UnLock();
            }
        }
    }
}