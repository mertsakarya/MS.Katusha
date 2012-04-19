using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Services;
using NLog;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace MS.Katusha.Services.Generators
{
    public class ConversationGenerator : IGenerator<Conversation>
    {
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;
        private readonly IConversationService _conversationService;
        private readonly List<Profile> _profiles;
        private int _total;

        private readonly static Logger Logger = LogManager.GetLogger("MS.Katusha.ConversationGenerator");


        public ConversationGenerator(IProfileService profileService, IUserService userService, IConversationService conversationService)
        {
            _profileService = profileService;
            _userService = userService;
            _conversationService = conversationService;
            _profiles = new List<Profile>(profileService.GetNewProfiles(p => p.Id > 0, out _total, 1, 200));
            _total = _profiles.Count;
        }

        public Conversation Generate(int extra = 0)
        {
#if DEBUG
            Logger.Info("Conversation START:");
#endif
            if (extra > 0) _total = extra;
            var message = new Conversation {Message = GeneratorHelper.RandomString(300, false), Subject = GeneratorHelper.RandomString(50, true)};
            if (GeneratorHelper.RND.Next(10) < 7)
                message.ReadDate = DateTime.Now;
            else {
                message.ReadDate = new DateTime(1900, 1, 1);
            }
            var num = GeneratorHelper.RND.Next(_total - 2) + 1;
            var from = _profiles[num];
            num = GeneratorHelper.RND.Next(_total - 1);
            var to = _profiles[num];
            if (from.Id == to.Id) return Generate();
            message.FromId = @from.Id;
            message.FromGuid = @from.Guid;
            message.FromName = @from.Name;
            message.FromPhotoGuid = @from.ProfilePhotoGuid;
            message.ToId = to.Id;
            message.ToGuid = to.Guid;
            message.ToName = to.Name;
            message.ToPhotoGuid = to.ProfilePhotoGuid;
            _conversationService.SendMessage(message);
#if DEBUG
            Logger.Info("Conversation END:");
#endif
            return message;
        }
    }
}
