using System;
using MS.Katusha.Domain.Raven.Entities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface ITokBoxRepositoryRavenDB
    {
        TokBoxSession GetSession(Guid profileGuid, string ip);
        TokBoxSession SetSession(Guid profileGuid, string ip, string sessionId);
        TokBoxSession UpdateSession(TokBoxSession tokBoxSession);
        void DeleteSession(TokBoxSession tokBoxSession);
    }
}