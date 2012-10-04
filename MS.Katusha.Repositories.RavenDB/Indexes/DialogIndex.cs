using System.Linq;
using MS.Katusha.Domain.Raven.Entities;
using Raven.Client.Indexes;

namespace MS.Katusha.Repositories.RavenDB.Indexes
{

    public class DialogIndex : AbstractIndexCreationTask<Conversation, DialogResult>
    {
        public DialogIndex()
        {

            Map = docs => from doc in docs
                select new DialogResult  {
                    FromId = ((long)doc.FromId),
                    ToId = ((long)doc.ToId),
                    Count = 1, 
                    UnreadCount = doc.ReadDate.Year < 2000 ? 1 : 0, 
                    LastConversationDate = doc.CreationDate
                };

            Reduce = results => from result in results
                group result by new { result.FromId, result.ToId }
                into g
                select new DialogResult {
                    FromId = g.Key.FromId,
                    ToId = g.Key.ToId,
                    LastConversationDate = g.Max(x=> x.LastConversationDate),
                    Count = g.Sum(x => x.Count),
                    UnreadCount = g.Sum(x => x.UnreadCount),
                };
        }
    }
}