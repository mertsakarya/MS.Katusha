using System.Collections.Generic;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepositoryDB _conversationRepository;

        public ConversationService(IConversationRepositoryDB conversationRepository)
        {
            _conversationRepository = conversationRepository;
        }

        public IEnumerable<Conversation> GetConversations(long profileId, int pageIndex, int pageSize, out int total) {
            var items = _conversationRepository.Query(p=>p.FromId == profileId || p.ToId == profileId, pageIndex, pageSize, out total, o=> o.CreationDate, p=> p.From, p=> p.To);
            return items;
        }
    }

}
