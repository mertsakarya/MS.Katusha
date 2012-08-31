using System.Web.Mvc;
using MS.Katusha.Configuration;
using Newtonsoft.Json;

namespace MS.Katusha.Web.Controllers
{
    public class ConfigController : Controller
    {
        [HttpGet]
        public void Display()
        {
            //var instance = KatushaConfigurationManager.Instance;
            //var settings = instance.GetSettings();
            //var result = new { instance.VirtualPath, instance.ConnectionString, settings.AdministratorMailAddress, settings.Ip, settings.MailViewFolder, settings.NotTrackedIpsByGoogleAnalytics, settings.Protocol };
            //Response.ContentType = "application/json";
            //Response.Write(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
