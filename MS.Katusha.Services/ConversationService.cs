using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Exceptions.Services;
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

        public IEnumerable<Conversation> GetMessages(long profileId, long fromId, out int total, int pageNo = 1, int pageSize = 20)
        {
            return _conversationRepositoryRaven.Query(q => (q.FromId == fromId && q.ToId == profileId) || (q.ToId == fromId && q.FromId == profileId), pageNo, pageSize, out total, o => o.CreationDate, false).ToList();
        }



        public IList<ConversationResult> GetConversations(long profileId, out int total, int pageNo = 1, int pageSize = 20)
        {
            return _conversationRepositoryRaven.MyConversations(profileId, out total, pageNo, pageSize);
        }

        public ConversationCountResult GetConversationStatistics(long profileId, MessageType messageType = MessageType.Received) { return _conversationRepositoryRaven.GetConversationStatistics(profileId, messageType); }

        public IEnumerable<Conversation> GetMessages(long profileId, MessageType messageType, out int total, int pageNo = 1, int pageSize = 20)
        {
            if (messageType == MessageType.Received) {
                return _conversationRepositoryRaven.Query(q => (q.ToId == profileId), pageNo, pageSize, out total, o => o.CreationDate, false).ToList();
            } else {
                return _conversationRepositoryRaven.Query(q => (q.FromId == profileId), pageNo, pageSize, out total, o => o.CreationDate, false).ToList();
            }
        }

        public void SendMessage(User user, Conversation message, bool force = false)
        {
            if(!force)
                if (user.Gender == (byte)Sex.Male)
                    if (user.Expires < DateTime.Now)
                        throw new KatushaNeedsPaymentException(user, ProductNames.MonthlyKatusha);
            message.Message = message.Message.Replace("\r\n", "<br />\r\n");
            var dbMessage = Mapper.Map<Domain.Entities.Conversation>(message);
            _conversationRepository.Add(dbMessage);
            message.Guid = dbMessage.Guid;
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

        public string ReadMessage(User user, long profileId, Guid messageGuid, bool force = false)
        {
            if (user.Gender == (byte)Sex.Male)
                if (user.Expires < DateTime.Now)
                    throw new KatushaNeedsPaymentException(user, ProductNames.MonthlyKatusha);


            var readTime = DateTime.Now;
            var ravenMessage = _conversationRepositoryRaven.SingleAttached(p => p.Guid == messageGuid && p.ToId == profileId);
            if (ravenMessage == null) return "";
            ravenMessage.ReadDate = readTime;
            _conversationRepositoryRaven.FullUpdate(ravenMessage);

            var message = _conversationRepository.SingleAttached(p => p.Guid == messageGuid && p.ToId == profileId);
            if (message == null) return "";
            message.ReadDate = readTime;
            _conversationRepository.FullUpdate(message);

            _notificationService.MessageRead(ravenMessage);
            return message.Message;
        }

        public void DeleteMessage(Guid messageGuid, bool softDelete = false) {
            var ravenMessage = _conversationRepositoryRaven.SingleAttached(p => p.Guid == messageGuid);
            if (ravenMessage == null) return;
            _conversationRepositoryRaven.Delete(ravenMessage);
            var message = _conversationRepository.SingleAttached(p => p.Guid == messageGuid);
            if (message == null) return;

            if(softDelete)
                _conversationRepository.SoftDelete(message);
            else
                _conversationRepository.Delete(message);
        }

        public bool HasPhotoGuid(Guid photoGuid) { return _conversationRepositoryRaven.Single(p => p.ToPhotoGuid == photoGuid || p.FromPhotoGuid == photoGuid) != null; }

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


        public IList<Dialog> GetDialogs(long profileId, out int total, int pageNo, int pageSize)
        {
            var list = _conversationRepositoryRaven.GetDialogs(profileId);
            total = list.Count;
            return list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
        }

        public IList<Conversation> GetConversation(long profileId, long withProfileId, out int total, int pageNo, int pageSize)
        {
            return _conversationRepositoryRaven.GetConversation(profileId, withProfileId, out total, pageNo, pageSize);
        }

        public IList<Domain.Entities.Conversation> GetMessagesByTime(int pageNo, DateTime dateTime, out int total, int pageSize)
        {
            total = _conversationRepository.Count(p => p.ModifiedDate > dateTime);
            return _conversationRepository.Query(p => p.ModifiedDate > dateTime, null, false);
        }
    }

}
