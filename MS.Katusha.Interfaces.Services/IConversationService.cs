using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Raven.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface IConversationService : IRestore<Domain.Entities.Conversation>
    {
        IEnumerable<Conversation> GetMessages(long profileId, long fromId, out int total, int pageNo = 1, int pageSize = 20);
        IList<ConversationResult> GetConversations(long profileId, out int total, int pageNo = 1, int pageSize = 20);
        void SendMessage(Conversation data);
        void ReadMessage(long id, Guid messageGuid);
    }
}
