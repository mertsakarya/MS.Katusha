using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MS.Katusha.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("SiteMapXml", "sitemap.xml", new { controller = "Home", action = "SiteMapXml" });
            routes.MapRoute("RobotsTxt", "robots.txt", new { controller = "Home", action = "RobotsTxt" });
            routes.MapRoute("Photo", "{controller}/Photo/{key}/{size}", new { action = "Photo" });
            routes.MapRoute("MakeProfilePhoto", "{controller}/MakeProfilePhoto/{key}/{photoGuid}", new { action = "MakeProfilePhoto" });
            routes.MapRoute("Download", "{controller}/Download/{key}/{size}", new { action = "Download" });

            routes.MapRoute(name: "Default", url: "{controller}/{action}/{key}", defaults: new { controller = "Home", action = "Index", key = UrlParameter.Optional } );
        }
    }
}