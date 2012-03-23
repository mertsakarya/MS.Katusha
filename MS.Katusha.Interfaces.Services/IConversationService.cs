using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface IConversationService
    {
        IEnumerable<Conversation> GetConversations(long profileId, int pageIndex, int pageSize, out int total);
    }
}
