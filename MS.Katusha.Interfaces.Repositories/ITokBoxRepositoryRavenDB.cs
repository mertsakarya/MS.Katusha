using System;
using MS.Katusha.Domain.Raven.Entities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface ITokBoxRepositoryRavenDB
    {
        TokBoxSession GetSession(Guid profileGuid);
        TokBoxSession SetSession(Guid profileGuid, string sessionId);
        TokBoxSession UpdateSession(TokBoxSession tokBoxSession);
        void DeleteSession(TokBoxSession tokBoxSession);
    }
}