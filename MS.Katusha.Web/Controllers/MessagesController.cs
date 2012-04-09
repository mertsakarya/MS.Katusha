using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Controllers.BaseControllers;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;
using MS.Katusha.Attributes;

namespace MS.Katusha.Web.Controllers
{
    [KatushaFilter(ExceptionView = "KatushaException", IsAuthenticated = false, MustHaveGender = false, MustHaveProfile = false)]
    public class MessagesController : KatushaController
    {
        private readonly IProfileService _profileService;
        private readonly IConversationService _conversationService;
        private const int PageSize = DependencyHelper.GlobalPageSize;

        public MessagesController(IProfileService profileService, IUserService userService, IConversationService conversationService)
            : base(userService)
        {
            _profileService = profileService;
            _conversationService = conversationService;
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult My(int? key = 1)
        {
            int total;
            var pageIndex = (key ?? 1);
            var messages = _conversationService.GetMessages(KatushaProfile.Id, out total, pageIndex);
            var messagesModel = Mapper.Map<IList<ConversationModel>>(messages);
            var messagesAsIPagedList = new StaticPagedList<ConversationModel>(messagesModel, pageIndex, PageSize, total);
            var model = new PagedListModel<ConversationModel> {List = messagesAsIPagedList};
            return View(model);
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Send(string key)
        {
            var to = _profileService.GetProfile(key);
            var model = new ConversationModel {
                                                ToId = to.Id, ToName = to.Name, ToPhotoGuid = to.ProfilePhotoGuid, ToGuid = to.Guid,
                                                FromId = KatushaProfile.Id, FromName = KatushaProfile.Name, FromPhotoGuid = KatushaProfile.ProfilePhotoGuid, FromGuid = KatushaProfile.Guid
            };
            return View(model);
        }

        [HttpPost]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Send(string key, ConversationModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var to = _profileService.GetProfile(key);
            var data = Mapper.Map<ConversationRaven>(model);

            data.ToId = to.Id;
            data.ToName = to.Name;
            data.ToGuid = to.Guid;
            data.ToPhotoGuid = to.ProfilePhotoGuid;

            data.FromId = KatushaProfile.Id;
            data.FromName = KatushaProfile.Name;
            data.FromGuid = KatushaProfile.Guid;
            data.FromPhotoGuid = KatushaProfile.ProfilePhotoGuid;

            data.ReadDate = new DateTime(1900, 1, 1, 0, 0, 0);
            _conversationService.SendMessage(data);
            return RedirectToAction("My");
        }

        [HttpGet]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public void Read(string key)
        {
            Guid guid;
            if (!User.Identity.IsAuthenticated || KatushaUser.Gender == 0 || !Guid.TryParse(key, out guid)) throw new HttpException(404, "Message not found!");
            _conversationService.ReadMessage(KatushaProfile.Id, guid);
        }
    }
}
