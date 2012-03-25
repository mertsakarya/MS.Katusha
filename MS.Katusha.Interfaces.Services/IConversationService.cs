using System.Collections.Generic;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface IConversationService
    {
        IEnumerable<Conversation> GetConversations(long profileId, int pageIndex, int pageSize, out int total);
    }
}
