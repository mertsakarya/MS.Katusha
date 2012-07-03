using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace MS.Katusha.Services
{
    public class ConversationService : IConversationService
    {
        private readonly INotificationService _notificationService;
        private readonly IConversationRepositoryDB _conversationRepository;
        private readonly IConversationRepositoryRavenDB _conversationRepositoryRaven;

        public ConversationService(INotificationService notificationService, IConversationRepositoryDB conversationRepository, IConversationRepositoryRavenDB conversationRepositoryRaven)
        {
            _notificationService = notificationService;
            _conversationRepository = conversationRepository;
            _conversationRepositoryRaven = conversationRepositoryRaven;
        }

        public IList<ConversationResult> GetConversations(long profileId, out int total, int pageNo = 1, int pageSize = 20)
        {
            return _conversationRepositoryRaven.MyConversations(profileId, out total, pageNo, pageSize);
        }

        public ConversationCountResult GetConversationStatistics(long profileId) { return _conversationRepositoryRaven.GetConversationStatistics(profileId); }

        public IEnumerable<Conversation> GetMessages(long profileId, long fromId, out int total, int pageNo = 1, int pageSize = 20)
        {
            return _conversationRepositoryRaven.Query(q => (q.FromId == fromId && q.ToId == profileId) || (q.ToId == fromId && q.FromId == profileId), pageNo, pageSize, out total, o => o.CreationDate, false).ToList();
        }

        public void SendMessage(Conversation message)
        {
            var dbMessage = Mapper.Map<Domain.Entities.Conversation>(message);
            _conversationRepository.Add(dbMessage);
            var ravenMessage = Mapper.Map<Conversation>(dbMessage);
            ravenMessage.FromGuid = message.FromGuid;
            ravenMessage.FromName = message.FromName;
            ravenMessage.FromPhotoGuid = message.FromPhotoGuid;
            ravenMessage.ToGuid = message.ToGuid;
            ravenMessage.ToName = message.ToName;
            ravenMessage.ToPhotoGuid = message.ToPhotoGuid;
            _conversationRepositoryRaven.Add(ravenMessage);
            _notificationService.MessageSent(ravenMessage);
        }

        public void ReadMessage(long profileId, Guid messageGuid)
        {
            var messageForRaven = _conversationRepositoryRaven.SingleAttached(p => p.Guid == messageGuid && p.ToId == profileId);
            if (messageForRaven == null) return;
            messageForRaven.ReadDate = DateTime.Now;
            _conversationRepositoryRaven.FullUpdate(messageForRaven);

            var message = _conversationRepository.SingleAttached(p => p.Guid == messageGuid && p.ToId == profileId);
            if (message == null) return;
            message.ReadDate = DateTime.Now;
            _conversationRepository.FullUpdate(message);
        }

        public IList<string> RestoreFromDB(Expression<Func<Domain.Entities.Conversation, bool>> filter, bool deleteIfExists = false)
        {
            var dbRepository = _conversationRepository;
            var ravenRepository = _conversationRepositoryRaven;

            var result = new List<string>();
            var items = dbRepository.Query(filter, null, false);
            foreach (var item in items) {
                try {
                    if (deleteIfExists) {
                        var p = ravenRepository.GetById(item.Id);
                        if (p != null)
                            ravenRepository.Delete(p);
                    }
                    var ravenItem = Mapper.Map<Conversation>(item);
                    ravenRepository.Add(ravenItem);
                } catch (Exception ex) {
                    result.Add(String.Format("{0} - {1}", item.Id, ex.Message));
                }
            }
            return result;
        }
    }

}
