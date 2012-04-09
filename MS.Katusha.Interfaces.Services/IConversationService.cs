using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Raven.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface IConversationService
    {
        IEnumerable<ConversationRaven> GetMessages(long profileId, out int total, int pageNo = 1, int pageSize = 20);
        void SendMessage(ConversationRaven data);
        void ReadMessage(long id, Guid messageGuid);
    }
}
