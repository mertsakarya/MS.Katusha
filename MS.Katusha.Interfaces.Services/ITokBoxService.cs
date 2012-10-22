using System;
using MS.Katusha.Domain.Raven.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface ITokBoxService
    {
        TokBoxSession GetSession(Guid profileGuid, string ip);
        TokBoxSession CreateSession(Guid profileGuid, string ip);
        string GetToken(TokBoxSession session);
        //void DeleteSession(TokBoxSession session);
    }
}
