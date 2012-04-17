using System;
using System.Collections.Generic;
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
        private readonly IVisitService _visitService;
        private readonly IConversationService _conversationService;
        private readonly IStateService _stateService;

        public UtilitiesController(IUserService userService, IProfileService profileService, IConfigurationService configurationService, 
            ISamplesService samplesService, IVisitService visitService, IConversationService conversationService, IStateService stateService)
            : base(userService, profileService, stateService)
        {
            _configurationService = configurationService;
            _samplesService = samplesService;
            _visitService = visitService;
            _conversationService = conversationService;
            _stateService = stateService;
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
        public void Restore(string key, bool delete = false)
        {
            IList<string> list;
            switch(key.ToLowerInvariant()) {
                case "profiles":
                    list = ProfileService.RestoreFromDB(null, delete);
                    break;
                case "visits":
                    list = _visitService.RestoreFromDB(null, delete);
                    break;
                case "conversations":
                    list = _conversationService.RestoreFromDB(null, delete);
                    break;
                case "states":
                    list = _stateService.RestoreFromDB(null, delete);
                    break;
                default:
                    list = new List<string> {"Unnkown parameter: " + key};
                    break;
            }
            if(list.Count > 0)
                foreach (var line in list) {
                    Response.Write(String.Format("({0}) <br />", line));                    
                }
            Response.Write("DONE!");
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
