using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using PagedList;

namespace MS.Katusha.Web.Controllers
{
    public class DialogResultModel : DialogResult { }
    public class DialogsModel {
        public PagedListModel<DialogResultModel> Dialogs { get; set; }
        public ConversationCountResult StatisticsReceived { get; set; }
        public ConversationCountResult StatisticsSent { get; set; }
    }

    [KatushaFilter(IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
    public class DialogsController : KatushaController
    {
        private readonly IProfileService _profileService;
        private readonly IConversationService _conversationService;
        private const int PageSize = DependencyConfig.GlobalPageSize;

        public DialogsController(IResourceService resourceService, IUserService userService, IProfileService profileService, IConversationService conversationService, IStateService stateService)
            : base(resourceService, userService, profileService, stateService, conversationService)
        {
            _profileService = profileService;
            _conversationService = conversationService;
        }

        public ActionResult Index(int? key)
        {
            int total;
            var pageIndex = 1; // (key ?? 1);
            var profileId = (key ?? KatushaProfile.Id);
            var dialogs = _conversationService.GetDialogs(profileId, out total, pageIndex, PageSize);
            return View(dialogs);
        }

        public ActionResult Conversation(long id, int? page)
        {
            int total;
            var pageIndex = (page ?? 1);
            var messages = _conversationService.GetConversation(KatushaProfile.Id, id, out total, pageIndex, PageSize);
            var messagesModel = Mapper.Map<IList<ConversationModel>>(messages);
            var messagesAsIPagedList = new StaticPagedList<ConversationModel>(messagesModel, pageIndex, PageSize, total);
            var model = new MessagesModel
            {
                MessageType = MessageType.Sent,
                Statistics = null,
                Conversations = new PagedListModel<ConversationModel> { List = messagesAsIPagedList, Total = total },
            };
            return View("Messages", model);
        }
    }
}
