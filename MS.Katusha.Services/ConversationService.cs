using System;
using System.Collections.Generic;
using System.Linq;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

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
            return _conversationRepository.Query(q => q.FromId == profileId || q.ToId == profileId, pageNo, pageSize, out total, o => o.CreationDate, p => p.From, p => p.To).ToList();
        }

        public void SendMessage(Conversation message)
        {
            _conversationRepository.Add(message);
            _conversationRepositoryRaven.Add(message);
        }

        public void ReadMessage(long profileId, Guid messageGuid)
        {
            var message = _conversationRepository.SingleAttached(p => p.Guid == messageGuid && p.ToId == profileId);
            if (message == null) return;
            message.ReadDate = DateTime.UtcNow;
            _conversationRepository.FullUpdate(message);
            var messageRaven = _conversationRepository.SingleAttached(p => p.Guid == messageGuid && p.ToId == profileId);
            if (messageRaven == null) return;
            messageRaven.ReadDate = DateTime.UtcNow;
            _conversationRepositoryRaven.FullUpdate(messageRaven);
        }

    }

}
