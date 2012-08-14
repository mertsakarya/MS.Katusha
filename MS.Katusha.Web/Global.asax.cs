using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MS.Katusha.Infrastructure;
using MS.Katusha.Repositories.RavenDB;
using MS.Katusha.Web.Helpers;
using Raven.Client.Document;
using DependencyHelper = MS.Katusha.Web.Helpers.DependencyHelper;

namespace MS.Katusha.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) { filters.Add(new HandleErrorAttribute()); }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Photo", "{controller}/Photo/{key}/{size}", new { action = "Photo" });
            //routes.MapRoute("SetFacet", "{controller}/SetFacet/{key}/{value}", new { action = "SetFacet" });
            //routes.MapRoute("DeletePhoto", "{controller}/DeletePhoto/{key}/{photoGuid}", new {action = "DeletePhoto"});
            routes.MapRoute("MakeProfilePhoto", "{controller}/MakeProfilePhoto/{key}/{photoGuid}", new { action = "MakeProfilePhoto" });
            routes.MapRoute("Download", "{controller}/Download/{key}/{size}", new { action = "Download" });

            //routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{key}",
            //    defaults: new { guid = RouteParameter.Optional }
            //    );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{key}",
                defaults: new { controller = "Home", action = "Index", key = UrlParameter.Optional }
                );
        }

        protected void Application_Start()
        {
            Context.Application.Lock();
            try {
                //Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
                Database.SetInitializer(new KatushaContextInitializer());

                ModelMetadataProviders.Current = new KatushaMetadataProvider();
                DependencyHelper.RegisterDependencies();
                //QuartzHelper.RegisterQuartz();
                MapperHelper.HandleMappings();
                AreaRegistration.RegisterAllAreas();

                ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
                ValueProviderFactories.Factories.Add(new KatushaJsonValueProviderFactory());
                
                RegisterGlobalFilters(GlobalFilters.Filters);
                RegisterRoutes(RouteTable.Routes);

                //BundleHelper.RegisterBundles();
                BundleTable.Bundles.EnableDefaultBundles();

                var store = DependencyResolver.Current.GetService<IKatushaRavenStore>() as DocumentStore;
                if (store == null) return;
                Glimpse.RavenDb.Profiler.AttachTo(store);
                Glimpse.RavenDb.Profiler.HideFields("PasswordHash", "PasswordSalt");
                Application["MyDocStore"] = store;
            } finally {
                Context.Application.UnLock();
            }
        }
   }
}