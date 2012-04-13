using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class VisitService : IVisitService
    {
        private readonly IVisitRepositoryDB _visitRepository;
        private readonly IVisitRepositoryRavenDB _visitRepositoryRaven;
        private readonly IProfileRepositoryRavenDB _profileRepositoryRaven;

        public VisitService(IVisitRepositoryDB visitRepository, IVisitRepositoryRavenDB visitRepositoryRaven, IProfileRepositoryRavenDB profileRepositoryRaven)
        {
            _visitRepository = visitRepository;
            _visitRepositoryRaven = visitRepositoryRaven;
            _profileRepositoryRaven = profileRepositoryRaven;
        }

        public void Visit(Profile visitorProfile, Profile profile)
        {
            if (visitorProfile != null && profile.Id != visitorProfile.Id) {
                var visit = _visitRepository.SingleAttached(p => p.ProfileId == profile.Id && p.VisitorProfileId == visitorProfile.Id);

                if (visit == null) {
                    _visitRepository.Add(new Visit { ProfileId = profile.Id, VisitorProfileId = visitorProfile.Id, VisitCount = 1 });
                } else {
                    visit.VisitCount++;
                    _visitRepository.FullUpdate(visit);
                }

                //TODO: Modify this with PATCH method of Raven Repository. http://ravendb.net/docs/client-api/partial-document-updates
                //_visitRepositoryRaven.Add(new Visit { ProfileId = profile.Id, VisitorProfileId = visitorProfile.Id, VisitCount = visit.VisitCount });

                var visitRaven = _visitRepositoryRaven.SingleAttached(p => p.ProfileId == profile.Id && p.VisitorProfileId == visitorProfile.Id);
                if (visitRaven == null) {
                    _visitRepositoryRaven.Add(new Visit { ProfileId = profile.Id, VisitorProfileId = visitorProfile.Id, VisitCount = 1 });
                } else {
                    visitRaven.VisitCount++;
                    _visitRepositoryRaven.FullUpdate(visitRaven);
                }
            }
        }

        public IEnumerable<Visit> GetVisitors(long profileId, out int total, int pageNo = 1, int pageSize = 20)
        {
            var items = _visitRepositoryRaven.Query(p => p.ProfileId == profileId, pageNo, pageSize, out total, o => o.ModifiedDate, false);
            var profile = _profileRepositoryRaven.GetById(profileId);
            foreach(var item in items) {
                var visitorProfile = _profileRepositoryRaven.GetById(item.VisitorProfileId);
                item.Profile = profile;
                item.VisitorProfile = visitorProfile;
            }
            //var items = _visitRepository.Query(p => p.ProfileId == profileId, pageNo, pageSize, out total, o => o.ModifiedDate, false, p => p.VisitorProfile);
            return items;
        }

    }
}
