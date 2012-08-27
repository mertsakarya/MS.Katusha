using System.Linq;
using MS.Katusha.Domain.Raven.Entities;
using Raven.Client.Indexes;

namespace MS.Katusha.Repositories.RavenDB.Indexes
{
    public class ConversationToCountIndex : AbstractIndexCreationTask<Conversation, ConversationCountResult>
    {
        public ConversationToCountIndex()
        {
            Map = docs => from doc in docs
                          select new ConversationCountResult {
                              ProfileId = doc.ToId,
                              Count = 1,
                              UnreadCount = (doc.ReadDate.Year < 2000) ? 1 : 0
                          };

            Reduce = results => from result in results
                                group result by new {result.ProfileId}
                                into g
                                select new ConversationCountResult {
                                    ProfileId = g.Key.ProfileId,
                                    Count = g.Sum(x => x.Count),
                                    UnreadCount = g.Sum(x => x.UnreadCount)
                                };
        }

    }
}