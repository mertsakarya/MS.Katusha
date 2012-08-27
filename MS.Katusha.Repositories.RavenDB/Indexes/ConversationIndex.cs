using System.Linq;
using MS.Katusha.Domain.Raven.Entities;
using Raven.Client.Indexes;

namespace MS.Katusha.Repositories.RavenDB.Indexes
{
    public class ConversationIndex : AbstractIndexCreationTask<Conversation, ConversationResult>
    {
        public ConversationIndex()
        {
            Map = docs => from doc in docs
                        select new ConversationResult  {
                            FromId = doc.FromId,
                            ToId = doc.ToId,
                            Subject = doc.Subject,
                            Count = 1,
                            UnreadCount = ((doc.ReadDate.Year < 2000) ? 1 : 0)
                        } ;

            Reduce = results => from result in results
                            group result by new {result.FromId, result.ToId, result.Subject}
                            into g
                            select new ConversationResult {
                                FromId = g.Key.FromId,
                                ToId = g.Key.ToId,
                                Subject = g.Key.Subject,
                                Count = g.Sum(x => x.Count),
                                UnreadCount = g.Sum(x => x.UnreadCount),
                            };
        }
    }
}