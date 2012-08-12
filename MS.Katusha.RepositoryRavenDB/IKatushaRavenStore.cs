using System.Collections.Generic;
using MS.Katusha.Domain.Raven.Entities;
using Raven.Abstractions.Commands;
using Raven.Client;

namespace MS.Katusha.Repositories.RavenDB
{
    public interface IKatushaRavenStore : IDocumentStore {
        void ClearRaven();
        void Create();
        void DeleteAll<T>();
        List<ICommandData> DeleteProfile(long profileId, Domain.Entities.Visit[] visits, Conversation[] messages);
        void Batch(List<ICommandData> list);
    }
}