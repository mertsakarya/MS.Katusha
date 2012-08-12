using MS.Katusha.Domain.Entities;
using Raven.Client;

namespace MS.Katusha.Repositories.RavenDB
{
    public interface IKatushaRavenStore : IDocumentStore {
        void ClearRaven();
        void Create();
        void DeleteAll<T>();
        void DeleteProfile(long profileId, Visit[] visits, Conversation[] messages);
    }
}