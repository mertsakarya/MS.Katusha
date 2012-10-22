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

        public TokBoxSession GetSession(Guid profileGuid, string ip)
        {
            var session = _tokBoxRepository.GetSession(profileGuid, ip);
            return session ?? CreateSession(profileGuid, ip);
        }

        public TokBoxSession CreateSession(Guid profileGuid, string ip)
        {
            var sessionId = _tokBox.CreateSession(ip);
            return sessionId == null ? null : _tokBoxRepository.SetSession(profileGuid, ip, sessionId);
        }

        public string GetToken(TokBoxSession session)
        {
            return _tokBox.GenerateToken(session.SessionId);
        }

        //public void DeleteSession(TokBoxSession session) { _tokBoxRepository.DeleteSession(session); }
    }
}