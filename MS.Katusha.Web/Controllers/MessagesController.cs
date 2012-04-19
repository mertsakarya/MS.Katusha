using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Controllers.BaseControllers;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;

namespace MS.Katusha.Web.Controllers
{
    [KatushaFilter(ExceptionView = "KatushaException", IsAuthenticated = false, MustHaveGender = false, MustHaveProfile = false)]
    public class MessagesController : KatushaController
    {
        private readonly IProfileService _profileService;
        private readonly IConversationService _conversationService;
        private const int PageSize = DependencyHelper.GlobalPageSize;

        public MessagesController(IUserService userService, IProfileService profileService, IConversationService conversationService, IStateService stateService)
            : base(userService, profileService, stateService, conversationService)
        {
            _profileService = profileService;
            _conversationService = conversationService;
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult List(string key, int? page = 1)
        {
            int total;
            var from = _profileService.GetProfile(key);
            var pageIndex = (page ?? 1);
            var messages = _conversationService.GetMessages(KatushaProfile.Id, from.Id, out total, pageIndex);
            var messagesModel = Mapper.Map<IList<ConversationModel>>(messages);
            var messagesAsIPagedList = new StaticPagedList<ConversationModel>(messagesModel, pageIndex, PageSize, total);
            var model = new PagedListModel<ConversationModel> {List = messagesAsIPagedList, Total = total};
            return View(model);
        }

        [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
        public ActionResult Conversations(int? key = 1)
        {
            int total;
            var pageIndex = (key ?? 1);
            var conversationResults = _conversationService.GetConversations(KatushaProfile.Id, out total, pageIndex, PageSize);

            //TODO: What I did here is terrible find another way.
            var instance = ConversationResultTypeConverter.GetInstance();
            instance.ProfileService = _profileService;

            var conversationResultsModel = Mapper.Map<IList<ConversationResultModel>>(conversationResults);
            //End /TODO

            var conversationResultsAsIPagedList = new StaticPagedList<ConversationResultModel>(conversationResultsModel, pageIndex, PageSize, total);
            var model = new PagedListModel<ConversationResultModel> { List = conversationResultsAsIPagedList, Total = total };
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
            var data = Mapper.Map<Conversation>(model);

            data.ToId = to.Id;
            data.ToName = to.Name;
            data.ToGuid = to.Guid;
            data.ToPhotoGuid = to.ProfilePhotoGuid;

            data.FromId = KatushaProfile.Id;
            data.FromName = KatushaProfile.Name;
            data.FromGuid = KatushaProfile.Guid;
            data.FromPhotoGuid = KatushaProfile.ProfilePhotoGuid;

            data.ReadDate = new DateTimeOffset(new DateTime(1900, 1, 1));
            _conversationService.SendMessage(data);
            return RedirectToAction("Conversations");
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
