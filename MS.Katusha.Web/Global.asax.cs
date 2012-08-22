using System.Data.Entity;
using System.Linq;
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
            routes.MapRoute("SiteMapXml", "sitemap.xml", new { controller = "Home", action = "SiteMapXml" });
            routes.MapRoute("RobotsTxt", "robots.txt", new { controller = "Home", action = "RobotsTxt" });
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
                MapperHelper.HandleMappings();
                AreaRegistration.RegisterAllAreas();

                ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
                ValueProviderFactories.Factories.Add(new KatushaJsonValueProviderFactory());
                
                RegisterGlobalFilters(GlobalFilters.Filters);
                RegisterRoutes(RouteTable.Routes);

                BundleScripts();

                //--------------------------------------------------------------------------


                var store = DependencyResolver.Current.GetService<IKatushaRavenStore>() as DocumentStore;
                if (store == null) return;
                Glimpse.RavenDb.Profiler.AttachTo(store);
                Glimpse.RavenDb.Profiler.HideFields("PasswordHash", "PasswordSalt");
                Application["MyDocStore"] = store;
            } finally {
                Context.Application.UnLock();
            }
        }

        private static void BundleScripts()
        { //BundleHelper.RegisterBundles();
            BundleTable.Bundles.EnableDefaultBundles();

            //--------------------------------------------------------------------------

            //var bundle = new Bundle("~/Scripts/js", new JsMinify());
            var cssBundle = new Bundle("~/Static/Content/css") {Orderer = new AsIsBundleOrderer()};
            cssBundle.AddFile("~/Static/Content/Fcbk.css");
            cssBundle.AddFile("~/Static/Content/Site.css");
            cssBundle.AddFile("~/Static/Content/PagedList.css");
            cssBundle.AddFile("~/Static/Content/mosaic.css");
            cssBundle.AddFile("~/Static/Content/bootstrap.min.css");
            cssBundle.AddFile("~/Static/Content/jquery.fileupload-ui.css");
            cssBundle.AddFile("~/Static/Content/bootstrap-image-gallery.min.css");
            cssBundle.AddFile("~/Static/Content/bootstrap-responsive.min.css");

            var bundle = new Bundle("~/Static/Scripts/js") {Orderer = new AsIsBundleOrderer()};
            bundle.AddFile("~/Static/Scripts/jquery.min-1.7.1.js", true);
            bundle.AddFile("~/Static/Scripts/jquery-ui-1.8.19.min.js", true);
            bundle.AddFile("~/Static/Scripts/bootstrap.min.js", true);
            bundle.AddFile("~/Static/Scripts/modernizr-2.5.3.js", true);
            bundle.AddFile("~/Static/Scripts/AjaxLogin.js", true);
            bundle.AddFile("~/Static/Scripts/knockout-2.1.0.js", true);
            bundle.AddFile("~/Static/Scripts/mosaic.1.0.1.js", true);
            bundle.AddFile("~/Static/Scripts/jquery.fcbkcomplete.js", true);
            bundle.AddFile("~/Static/Scripts/jquery.validate.min.js", true);
            bundle.AddFile("~/Static/Scripts/jquery.unobtrusive-ajax.js", true);

            bundle.AddFile("~/Static/Scripts/tmpl.min.js", true);
            bundle.AddFile("~/Static/Scripts/canvas-to-blob.min.js", true);
            bundle.AddFile("~/Static/Scripts/load-image.min.js", true);

            bundle.AddFile("~/Static/Scripts/bootstrap-image-gallery.min.js", true);

            bundle.AddFile("~/Static/Scripts/jquery.iframe-transport.js", true);
            bundle.AddFile("~/Static/Scripts/jquery.fileupload.js", true);
            bundle.AddFile("~/Static/Scripts/jquery.fileupload-ip.js", true);
            bundle.AddFile("~/Static/Scripts/jquery.fileupload-ui.js", true);

            bundle.AddFile("~/Static/Scripts/locale.js", true);
            bundle.AddFile("~/Static/Scripts/main.js", true);

            BundleTable.Bundles.Add(cssBundle);
            BundleTable.Bundles.Add(bundle);
        }

        protected void Application_BeginRequest()
        {
            //var protocol = KatushaConfigurationManager.Instance.GetSettings().Protocol;
            //if (protocol != "https") return;
            //var requestProtocol = Request.Headers["X-Forwarded-Proto"];
            //requestProtocol = String.IsNullOrEmpty(requestProtocol) ? ((Request.ServerVariables["HTTPS"].ToLowerInvariant() == "on") ? "https" : "http") : requestProtocol.ToLowerInvariant();
            //if (requestProtocol == "https") return;
            //var uri = Context.Request.Url.GetComponents(UriComponents.Host | UriComponents.PathAndQuery, UriFormat.UriEscaped);
            ////UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped);
            //Response.Redirect("https://" + uri);
        }
   }
}