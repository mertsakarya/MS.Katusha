using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Infrastructure.Exceptions.Web;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Controllers
{
    [KatushaFilter(ExceptionView = "KatushaException", IsAuthenticated = false, MustHaveGender = false, MustHaveProfile = false)]
    public class PhotosController : KatushaController
    {
        private readonly IProfileService _profileService;
        private readonly IPhotosService _photosService;

        public PhotosController(IUserService userService, IProfileService profileService, IPhotosService photosService, IStateService stateService, IConversationService conversationService)
            : base(userService, profileService, stateService, conversationService)
        {
            _profileService = profileService;
            _photosService = photosService;
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult List()
        {
            var profile = _profileService.GetProfile(KatushaProfile.Id, null, p=>p.Photos);
            ViewBag.SameProfile = true;
            var model = Mapper.Map<ProfileModel>(profile);
            return View(model);
        }

        [System.Web.Http.HttpGet]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public void Delete(string key, string photoGuid)
        {
            if (!IsKeyForProfile(key)) throw new HttpException(404, "Photo not found!");
            _photosService.DeletePhoto(KatushaProfile.Id, Guid.Parse(photoGuid), Server.MapPath(ConfigurationManager.AppSettings["Photos_Folder"]));
        }

        [HttpGet]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public void MakeProfilePhoto(string key, string photoGuid)
        {
            if (!IsKeyForProfile(key)) throw new HttpException(404, "Photo not found!");
            _photosService.MakeProfilePhoto(KatushaProfile.Id, Guid.Parse(photoGuid));
        }

        [HttpPost, ValidateInput(false)]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult UploadFiles(string key)
        {
            if (!IsKeyForProfile(key)) throw new KatushaNotAllowedException(KatushaProfile, KatushaUser, key);
            string description = Request.Form["description[]"];
            if (String.IsNullOrWhiteSpace(description))
                description = "";
            var list = new List<ViewDataUploadFilesResult>();
            foreach (string file in Request.Files) {
                var hpf = Request.Files[file];
                var viewDataUploadResult = _photosService.AddPhoto(KatushaProfile.Id, description, Server.MapPath(ConfigurationManager.AppSettings["Photos_Folder"]), hpf);
                if (hpf != null) if (hpf.ContentLength != 0 && hpf.FileName != null) list.Add(viewDataUploadResult);
            }
            return new ContentResult {
                Content = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(list),
                ContentType = "application/json"
            };
        }
    }
}
