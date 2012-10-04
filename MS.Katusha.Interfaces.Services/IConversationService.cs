using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace MS.Katusha.Interfaces.Services
{
    public interface IConversationService : IRestore<Domain.Entities.Conversation>
    {
        ConversationCountResult GetConversationStatistics(long profileId, MessageType messageType = MessageType.Received);
        IEnumerable<Conversation> GetMessages(long profileId, long fromId, out int total, int pageNo = 1, int pageSize = 20);
        IEnumerable<Conversation> GetMessages(long profileId, MessageType messageType, out int total, int pageNo = 1, int pageSize = 20);

        IList<ConversationResult> GetConversations(long profileId, out int total, int pageNo = 1, int pageSize = 20);
        void SendMessage(User user, Conversation data,  bool force = false);
        string ReadMessage(User user, long profileId, Guid messageGuid, bool force = false);
        void DeleteMessage(Guid messageGuid, bool softDelete = false);
        bool HasPhotoGuid(Guid photoGuid);

        IList<Dialog> GetDialogs(long profileId, out int total, int pageNo, int pageSize);
        IList<Conversation> GetConversation(long profileId, long withProfileId, out int total, int pageNo, int pageSize);
    }
}
