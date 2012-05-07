using Raven.Client;

namespace MS.Katusha.Repositories.RavenDB
{
    public interface IKatushaRavenStore : IDocumentStore {
        void ClearRaven();
        void Create();
        void DeleteAll<T>();
    }
}