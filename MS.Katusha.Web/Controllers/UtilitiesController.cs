using System;
using System.Linq;
using System.Web.Mvc;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Controllers.BaseControllers;

namespace MS.Katusha.Web.Controllers
{
    public class UtilitiesController : KatushaController
    {
        private readonly IConfigurationService _configurationService;

        public UtilitiesController(IUserService service, IConfigurationService configurationService)
            : base(service)
        {
            _configurationService = configurationService;
        }

        [HttpGet]
        public void InitConfiguration()
        {
            Response.ContentType = "text/plain";
            Response.Write(_configurationService.ResetDatabaseResources().Aggregate("", (current, line) => current + (line + "\r\n")));
        }

        [HttpGet]
        public void GenerateProfiles(string key)
        {
            Response.ContentType = "text/plain";
            int count;
            if (!int.TryParse(key, out count)) {
                Response.Write("Wrong count!");
            }  else {
                _configurationService.GenerateRandomUserAndProfile(count);
                Response.Write(String.Format("({0}) Users and profiles are created!", count));
            }
        }
    }
}
