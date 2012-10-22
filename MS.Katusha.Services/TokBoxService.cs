using System;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class TokBoxService : ITokBoxService
    {
        private readonly ITokBoxRepositoryRavenDB _tokBoxRepository;
        private readonly TokBox _tokBox;

        public TokBoxService(ITokBoxRepositoryRavenDB tokBoxRepository)
        {
            _tokBox = new TokBox();
            _tokBoxRepository = tokBoxRepository;
        }

        public TokBoxSession GetSession(Guid profileGuid, string location)
        {
            var session = _tokBoxRepository.GetSession(profileGuid);
            if (session != null) return session;
            return CreateSession(profileGuid, location);
        }

        public TokBoxSession CreateSession(Guid profileGuid, string location)
        {
            var sessionId = _tokBox.CreateSession(location);
            if (sessionId == null) return null;
            var session = new TokBoxSession { ProfileGuid = profileGuid, SessionId = sessionId };
            return _tokBoxRepository.UpdateSession(session);
        }

        public string GetToken(TokBoxSession session)
        {
            return _tokBox.GenerateToken(session.SessionId);
        }

        public void DeleteSession(TokBoxSession session) { _tokBoxRepository.DeleteSession(session);
        }
    }
}