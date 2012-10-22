using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Helpers;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;

namespace MS.Katusha.Web.Controllers
{
    [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
    public class MessagesController : KatushaController
    {
        private readonly IProfileService _profileService;
        private readonly IConversationService _conversationService;
        private const int PageSize = DependencyConfig.GlobalPageSize;

        public MessagesController(IResourceService resourceService, IUserService userService, IProfileService profileService, IConversationService conversationService, IStateService stateService, ITokBoxService tokBoxService)
            : base(resourceService, userService, profileService, stateService, conversationService, tokBoxService)
        {
            _profileService = profileService;
            _conversationService = conversationService;
        }

        public ActionResult Received(int? key = 1) {
            return GetMessages(MessageType.Received,  key);
        }

        public ActionResult Sent(int? key = 1)
        {
            return GetMessages(MessageType.Sent, key);
        }

        private ActionResult GetMessages(MessageType messageType, int? key)
        {
            int total;
            var pageIndex = (key ?? 1);
            var statistics = _conversationService.GetConversationStatistics(KatushaProfile.Id, messageType);
            var messages = _conversationService.GetMessages(KatushaProfile.Id, messageType, out total, pageIndex, PageSize);
            var messagesModel = Mapper.Map<IList<ConversationModel>>(messages);
            var messagesAsIPagedList = new StaticPagedList<ConversationModel>(messagesModel, pageIndex, PageSize, total);
            var model = new MessagesModel {
                MessageType = messageType,
                Statistics = statistics,
                Conversations = new PagedListModel<ConversationModel> {List = messagesAsIPagedList, Total = total},
            };
            return View("Index", model);
        }

        [KatushaNeedsPayment(Product = ProductNames.MonthlyKatusha)]
        public ActionResult Send(string key, string subject="")
        {
            var to = _profileService.GetProfile(key);
            var model = new ConversationModel {
                ToId = to.Id, ToName = to.Name, ToPhotoGuid = to.ProfilePhotoGuid, ToGuid = to.Guid,
                FromId = KatushaProfile.Id, FromName = KatushaProfile.Name, FromPhotoGuid = KatushaProfile.ProfilePhotoGuid, FromGuid = KatushaProfile.Guid
            };
            return ContextDependentView(model, "Send");
            //return View(model);
        }

        [HttpPost]
        [KatushaNeedsPayment(Product = ProductNames.MonthlyKatusha, IsJson = true)]
        public JsonResult JsonSend(string key, ConversationModel model)
        {
            return (JsonResult) _Send(key, model, Json(new { success = true, message = "Your message has been sent." }), Json(new { errors = GetErrorsFromModelState() }));
        }

        [HttpPost]
        [KatushaNeedsPayment(Product = ProductNames.MonthlyKatusha)]
        public ActionResult Send(string key, ConversationModel model)
        {
            return _Send(key, model, RedirectToAction("Received"), View(model));
        }

        private ActionResult _Send(string key, ConversationModel model, ActionResult successResult, ActionResult failResult)
        {
            if (!ModelState.IsValid) return failResult;
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

            data.ReadDate = new DateTime(1900, 1, 1);
            _conversationService.SendMessage(KatushaUser, data);
            return successResult;
        }

        [HttpPost]
        [KatushaNeedsPayment(Product = ProductNames.MonthlyKatusha, IsJson = true)]
        public JsonResult Read(string key)
        {
            Guid guid;
            if (!User.Identity.IsAuthenticated || KatushaUser.Gender == 0 || !Guid.TryParse(key, out guid)) throw new HttpException(404, "Message not found!");
            var message = _conversationService.ReadMessage(KatushaUser, KatushaProfile.Id, guid);
            return Json(new {message = message});
        }
    }
}
