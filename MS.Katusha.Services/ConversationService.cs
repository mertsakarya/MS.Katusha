using System;
using System.Collections.Generic;
using System.Linq;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace MS.Katusha.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepositoryDB _conversationRepository;
        private readonly IConversationRepositoryRavenDB _conversationRepositoryRaven;

        public ConversationService(IConversationRepositoryDB conversationRepository, IConversationRepositoryRavenDB conversationRepositoryRaven)
        {
            _conversationRepository = conversationRepository;
            _conversationRepositoryRaven = conversationRepositoryRaven;
        }

        public IEnumerable<Conversation> GetMessages(long profileId, out int total, int pageNo = 1, int pageSize = 20)
        {
            return _conversationRepositoryRaven.Query(q => q.FromId == profileId || q.ToId == profileId, pageNo, pageSize, out total, o => o.CreationDate).ToList();
        }

        public void SendMessage(Conversation message)
        {
            var dbMessage = AutoMapper.Mapper.Map<MS.Katusha.Domain.Entities.Conversation>(message);
            _conversationRepository.Add(dbMessage);
            var ravenMessage = AutoMapper.Mapper.Map<Conversation>(dbMessage);
            ravenMessage.FromGuid = message.FromGuid;
            ravenMessage.FromName = message.FromName;
            ravenMessage.FromPhotoGuid = message.FromPhotoGuid;
            ravenMessage.ToGuid = message.ToGuid;
            ravenMessage.ToName = message.ToName;
            ravenMessage.ToPhotoGuid = message.ToPhotoGuid;
            _conversationRepositoryRaven.Add(ravenMessage);
        }

        public void ReadMessage(long profileId, Guid messageGuid)
        {
            var messageForRaven = _conversationRepositoryRaven.SingleAttached(p => p.Guid == messageGuid && p.ToId == profileId);
            if (messageForRaven == null) return;
            messageForRaven.ReadDate = DateTime.UtcNow;
            _conversationRepositoryRaven.FullUpdate(messageForRaven);

            var message = _conversationRepository.SingleAttached(p => p.Guid == messageGuid && p.ToId == profileId);
            if (message == null) return;
            message.ReadDate = DateTime.UtcNow;
            _conversationRepository.FullUpdate(message);
        }

    }

}
