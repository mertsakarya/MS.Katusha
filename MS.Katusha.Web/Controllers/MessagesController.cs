using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Controllers.BaseControllers;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;
using MS.Katusha.Domain.Entities;
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
            var toModel = Mapper.Map<ProfileModel>(to);
            var from = Mapper.Map<ProfileModel>(KatushaProfile);
            var model = new ConversationModel {To = toModel, ToId = toModel.Id, FromId = from.Id, From = from};
            return View(model);
        }

        [HttpPost]
        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Send(string key, ConversationModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var to = _profileService.GetProfile(key);
            var data = Mapper.Map<Conversation>(model);
            data.ToId = to.Id;
            data.FromId = KatushaProfile.Id;
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
