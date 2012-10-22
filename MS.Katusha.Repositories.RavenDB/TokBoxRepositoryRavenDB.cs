using System;
using System.Linq;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Interfaces.Repositories;

namespace MS.Katusha.Repositories.RavenDB
{
    public class TokBoxRepositoryRavenDB : ITokBoxRepositoryRavenDB
    {
        private readonly IKatushaRavenStore _documentStore;
        public TokBoxRepositoryRavenDB(IKatushaRavenStore documentStore) { _documentStore = documentStore; }

        public TokBoxSession GetSession(Guid profileGuid, string ip)
        {
            using (var session = _documentStore.OpenSession()) {
                return session.Query<TokBoxSession>().FirstOrDefault(p => p.ProfileGuid == profileGuid && p.IP == ip);
                //.AsProjection<Profile>()
            }
        }
        public TokBoxSession SetSession(Guid profileGuid, string ip, string sessionId) { 
            var tokBoxSession = GetSession(profileGuid, ip);
            if (tokBoxSession == null) {
                using (var session = _documentStore.OpenSession()) {
                    tokBoxSession = new TokBoxSession { LastModified = DateTime.Now, ProfileGuid = profileGuid, IP = ip, SessionId = sessionId };
                    session.Store(tokBoxSession);
                    //session.Advanced.GetMetadataFor(tokBoxSession)["Raven-Expiration-Date"] = new RavenJValue(DateTime.Now.AddMinutes(1).ToUniversalTime());
                    session.SaveChanges();
                }
            } else {
                tokBoxSession.SessionId = sessionId;
                UpdateSession(tokBoxSession);
            }
            
            return tokBoxSession;
        }

        public TokBoxSession UpdateSession(TokBoxSession tokBoxSession)
        {
            tokBoxSession.LastModified = DateTime.Now;
            using (var session = _documentStore.OpenSession()) {
                session.Store(tokBoxSession);
                session.SaveChanges();
            }
            return tokBoxSession;
        }

        public void DeleteSession(TokBoxSession tokBoxSession)
        {
            using (var session = _documentStore.OpenSession()) {
                session.Delete(tokBoxSession);
                session.SaveChanges();
            }
        }
    }
}
