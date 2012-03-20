using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Photo",        "{controller}/Photo/{key}/{size}", new { action = "Photo" }); 
            routes.MapRoute("DeletePhoto",  "{controller}/DeletePhoto/{key}/{photoGuid}", new { action = "DeletePhoto" });
            routes.MapRoute("Download",     "{controller}/Download/{key}/{size}", new { action = "Download" });


            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{key}",
                defaults: new { guid = RouteParameter.Optional }
            );
 
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{key}",
                defaults: new { controller = "Home", action = "Index", key = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            //Database.DefaultConnectionFactory = new SqlConnectionFactory(@"Data Source=localhost;Initial Catalog=Test;Integrated Security=True;Pooling=False");
            ModelMetadataProviders.Current = new KatushaMetadataProvider();
            DependencyHelper.RegisterDependencies();
            MapperHelper.HandleMappings();
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            //BundleTable.Bundles.RegisterTemplateBundles();
            BundleTable.Bundles.EnableDefaultBundles();


            //Bundle debugScripts = new Bundle("~/DebugScripts", new NoTransform("text/javascript"));
            //debugScripts.AddDirectory("~/Scripts/Debug", "*.js");
            //BundleTable.Bundles.Add(debugScripts);

            //Bundle productionScripts = new Bundle("~/ProductionScripts", new NoTransform("text/javascript"));
            //productionScripts.AddDirectory("~/Scripts/Minified", "*.js");
            //BundleTable.Bundles.Add(productionScripts);

        }
    }
}