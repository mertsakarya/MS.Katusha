using System.Collections.Generic;
using System.Linq;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Base;
using MS.Katusha.Repositories.RavenDB.Indexes;
using Raven.Client.Linq;

namespace MS.Katusha.Repositories.RavenDB
{
    public class ConversationRepositoryRavenDB : BaseGuidRepositoryRavenDB<Conversation>, IConversationRepositoryRavenDB
    {
        public ConversationRepositoryRavenDB(IKatushaRavenStore documentStore)
            : base(documentStore) { }


        public IList<ConversationResult> MyConversations(long profileId, out int total, int pageNo = 1, int pageSize = 20)
        {
            using (var session = DocumentStore.OpenSession()) {
                RavenQueryStatistics stats;
                var query = session.Query<ConversationResult, ConversationIndex>().Statistics(out stats).Where(p=> p.ToId == profileId || p.FromId == profileId).Skip((pageNo - 1) * pageSize).Take( pageSize ).ToList();
                total = stats.TotalResults;
                return query;
                //.AsProjection<Profile>()
            }
        }

        public ConversationCountResult GetConversationStatistics(long profileId, MessageType messageType = MessageType.Received)
        {
            using (var session = DocumentStore.OpenSession()) {
                if(messageType == MessageType.Received)
                    return session.Query<ConversationCountResult, ConversationToCountIndex>().FirstOrDefault(p => p.ProfileId == profileId);
                return session.Query<ConversationCountResult, ConversationFromCountIndex>().FirstOrDefault(p => p.ProfileId == profileId);
            }
        }


        public IList<Dialog> GetDialogs(long profileId)
        {
            using (var session = DocumentStore.OpenSession())
            {
                var dict = new Dictionary<long, Dialog>();
                foreach(var d in session.Query<DialogResult, DialogIndex>().Where(p => p.ToId == profileId || p.FromId == profileId)) {
                    MessageType messageType;
                    long id;
                    if(d.ToId == profileId) {
                        messageType = MessageType.Received;
                        id = d.FromId;
                    } else {
                        messageType = MessageType.Sent;
                        id = d.ToId;
                    }
                    var contains = dict.ContainsKey(id);
                    var item = (contains) ? dict[id] : new Dialog { ProfileId = id, LastReceivedDate = new System.DateTime(1900, 1,1), LastSentDate = new System.DateTime(1900, 1,1) };
                    item.Count += d.Count;
                    if (messageType == MessageType.Received) {
                        item.LastReceivedDate = d.LastConversationDate;
                        item.UnreadReceivedCount += d.UnreadCount;
                    } else {
                        item.LastSentDate = d.LastConversationDate;
                        item.UnreadSentCount += d.UnreadCount;
                    }
                    if (!contains) {
                        dict.Add(id, item);
                    } else {
                        dict[id] = item;
                    }
                }
                var list = new List<Dialog>(dict.Count);
                foreach (var d in dict) {
                    list.Add(d.Value);
                }
                return list;
            }
        }

        public IList<Conversation> GetConversation(long profileId, long withProfileId, out int total, int pageNo, int pageSize)
        {
            using (var session = DocumentStore.OpenSession())
            {
                RavenQueryStatistics stats;
                var query = session.Query<Conversation>().Statistics(out stats)
                    .Where(p => (p.ToId == profileId && p.FromId == withProfileId) || (p.ToId == withProfileId && p.FromId == profileId))
                    .OrderByDescending(p=>p.CreationDate)
                    .Skip((pageNo - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                total = stats.TotalResults;
                return query;
            }
        }
    }
}