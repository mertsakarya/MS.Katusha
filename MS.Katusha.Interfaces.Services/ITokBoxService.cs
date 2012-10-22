using System;
using MS.Katusha.Domain.Raven.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface ITokBoxService
    {
        TokBoxSession GetSession(Guid profileGuid, string location);
        TokBoxSession CreateSession(Guid profileGuid, string location);
        string GetToken(TokBoxSession session);
        void DeleteSession(TokBoxSession session);
    }
}
