using System;
using System.Linq;
using System.Web.Mvc;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Services;
using MS.Katusha.Web.Controllers.BaseControllers;

namespace MS.Katusha.Web.Controllers
{
    public class UtilitiesController : KatushaController
    {
        private readonly IConfigurationService _configurationService;
        private readonly ISamplesService _samplesService;

        public UtilitiesController(IUserService userService, IConfigurationService configurationService, ISamplesService samplesService, IStateService stateService)
            : base(userService, stateService)
        {
            _configurationService = configurationService;
            _samplesService = samplesService;
        }

        [HttpGet]
        public void InitConfiguration()
        {
            Response.ContentType = "text/plain";
            var result = _configurationService.ResetDatabaseResources().Aggregate("", (current, line) => current + (line + "\r\n"));
            if (String.IsNullOrWhiteSpace(result)) result = "DONE!";
            Response.Write(result);
        }

        [HttpGet]
        public void GenerateProfiles(string key)
        {
            int extra;
            var count = GetValues(key, out extra);
            if (count <= 0) return;
            _samplesService.GenerateRandomUserAndProfile(count, extra);
            Response.Write(String.Format("({0}) items are created with extra {1}!", count, extra));
        }

        [HttpGet]
        public void GenerateConversations(string key)
        {
            int extra;
            var count = GetValues(key, out extra);
            if (count <= 0) return;
            _samplesService.GenerateRandomConversation(count, extra);
            Response.Write(String.Format("({0}) items are created with extra {1}!", count, extra));
        }

        [HttpGet]
        public void GenerateVisits(string key)
        {
            int extra;
            var count = GetValues(key, out extra);
            if (count <= 0) return;
            _samplesService.GenerateRandomVisit(count, extra);
            Response.Write(String.Format("({0}) items are created with extra {1}!", count, extra));
        }

        private int GetValues(string key, out int extra)
        {
            Response.ContentType = "text/plain";
            var count = 0;
            if (!int.TryParse(key, out count)) Response.Write("Wrong count!");
            var str = (Request.QueryString["extra"]);
            if (!int.TryParse(str, out extra)) extra = 0;
            return count;
        }

    }
}
