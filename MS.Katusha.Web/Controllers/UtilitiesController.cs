using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Services;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;

namespace MS.Katusha.Web.Controllers
{
    public class UtilitiesController : KatushaController
    {
        private readonly ISamplesService _samplesService;
        private readonly IVisitService _visitService;
        private readonly IConversationService _conversationService;
        private readonly IPhotosService _photosService;
        private readonly IUtilityService _utilityService;

        public UtilitiesController(IResourceService resourceService, IUserService userService, IProfileService profileService, 
            ISamplesService samplesService, IVisitService visitService, IConversationService conversationService, IStateService stateService,
            IPhotosService photosService, IUtilityService utilityService
            )
            : base(resourceService, userService, profileService, stateService, conversationService)
        {
           _samplesService = samplesService;
            _visitService = visitService;
            _conversationService = conversationService;
            _photosService = photosService;
            _utilityService = utilityService;
        }

        [HttpGet]
        public ActionResult Index() { return View(); }

        [HttpGet]
        public void InitConfiguration()
        {
            Response.ContentType = "text/plain";
            var result = _utilityService.ResetDatabaseResources().Aggregate("", (current, line) => current + (line + "\r\n"));
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
                    list = StateService.RestoreFromDB(null, delete);
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

        [HttpGet]
        public ActionResult Photos(string key)
        {
            int total;
            int pageNo;
            if (!int.TryParse(key, out pageNo)) {
                pageNo = 1;
            }

            var list = _photosService.Dir(DependencyHelper.PhotosFolder, out total, pageNo, DependencyHelper.GlobalPageSize);
            var pagedList = new StaticPagedList<Guid>(list, pageNo, DependencyHelper.GlobalPageSize, total);
            var photoGuids = new PagedListModel<Guid> { List = pagedList, Total = total };
            var dictionaryPhotos = new Dictionary<Guid, PhotoModel>();
            var dictionaryProfiles = new Dictionary<Guid, ProfileModel>();
            foreach (var guid in list) {
                var photo = _photosService.GetByGuid(guid);
                if (photo != null) {
                    dictionaryPhotos.Add(guid, Mapper.Map<PhotoModel>(photo));
                    dictionaryProfiles.Add(guid, Mapper.Map<ProfileModel>(ProfileService.GetProfile(photo.ProfileId)));
                }
            }
            var model = new UtilitiesPhotosModel { PhotoGuids = photoGuids, Photos = dictionaryPhotos, Profiles = dictionaryProfiles };
            return View(model);
        }

        [HttpGet]
        public void CheckPhotos()
        {
            var path = DependencyHelper.PhotosFolder;
            List<string> list = _photosService.CheckPhotos(path);
            list.AddRange(_photosService.CheckPhotoFiles(path));
            list.AddRange(_photosService.CheckProfilePhotos(path));
            foreach (var line in list) {
                Response.Write(line + "<br/>");
            }
            Response.Write("<hr/>FINISHED!");
        }

        private int GetValues(string key, out int extra)
        {
            Response.ContentType = "text/plain";
            int count;
            if (!int.TryParse(key, out count)) Response.Write("Wrong count!");
            var str = (Request.QueryString["extra"]);
            if (!int.TryParse(str, out extra)) extra = 0;
            return count;
        }

        public void RegisterRaven()
        {
            _utilityService.RegisterRaven();
            Response.Write("Done!");
        }

        public void ClearDatabase()
        {
            _utilityService.ClearDatabase(DependencyHelper.PhotosFolder);
            Response.Write("Done!");
        }
    }
}
