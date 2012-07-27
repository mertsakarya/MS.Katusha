using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IConversationService : IRestore<Domain.Entities.Conversation>
    {
        ConversationCountResult GetConversationStatistics(long profileId, MessageType messageType = MessageType.Received);
        IEnumerable<Conversation> GetMessages(long profileId, long fromId, out int total, int pageNo = 1, int pageSize = 20);
        IEnumerable<Conversation> GetMessages(long profileId, MessageType messageType, out int total, int pageNo = 1, int pageSize = 20);

        IList<ConversationResult> GetConversations(long profileId, out int total, int pageNo = 1, int pageSize = 20);
        void SendMessage(Conversation data);
        void ReadMessage(long profileId, Guid messageGuid);
        void DeleteMessage(Guid messageGuid, bool softDelete = false);
    }
}
