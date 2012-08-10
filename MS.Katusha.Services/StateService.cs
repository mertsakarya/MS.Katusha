using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Services
{
    public class StateService : IStateService
    {
        private readonly IStateRepositoryDB _stateRepository;
        private readonly IStateRepositoryRavenDB _stateRepositoryRaven;
        private readonly IVisitService _visitService;
        private readonly IProfileService _profileService;
        private readonly IConversationService _conversationService;
        private static readonly TimeSpan OnlineInterval = new TimeSpan(0, 0, 10, 0); 

        public StateService(IStateRepositoryDB stateRepository, IStateRepositoryRavenDB stateRepositoryRaven, IVisitService visitService, IProfileService profileService, IConversationService conversationService)
        {
            _stateRepository = stateRepository;
            _stateRepositoryRaven = stateRepositoryRaven;
            _visitService = visitService;
            _profileService = profileService;
            _conversationService = conversationService;
        }

        public PingResult Ping(Profile profile) {
            if(profile == null) return null;
            try {
                var lastVisitTime = _stateRepository.UpdateStatus(profile);
                var state = Mapper.Map<State>(profile);
                _stateRepositoryRaven.UpdateStatus(state);
                var visits = _visitService.GetVisitorsSinceLastVisit(profile.Id, lastVisitTime);
                var conversations = _conversationService.GetConversationStatistics(profile.Id);
                return new PingResult {Visits = visits, Conversations = conversations};
            } catch {
                return null;
            }
        }

        public IEnumerable<State> OnlineProfiles(byte sex, out int total, int pageNo = 1, int pageSize = 20)
        {
            var dt = DateTime.Now - OnlineInterval;
            IList<State> retval = null;
            total = 0;
            if (sex == 0)
                retval = _stateRepositoryRaven.Query(p => p.LastOnline > dt, pageNo, pageSize, out total, p => p.LastOnline, false);
            else if (sex <= (byte)Sex.MAX)
                retval = _stateRepositoryRaven.Query(p => p.LastOnline > dt && p.Gender == sex, pageNo, pageSize, out total, p => p.LastOnline, false);
            return retval;
        }

        public IList<string> RestoreFromDB(Expression<Func<State, bool>> filter, bool deleteIfExists = false)
        {
            var dbRepository = _stateRepository;
            var ravenRepository = _stateRepositoryRaven;

            var result = new List<string>();
            int total;
            var items = dbRepository.Query(filter, 1, int.MaxValue, out total, p=>p.Id, true);
            foreach (var item in items) {
                try {
                    if (deleteIfExists) {
                        var pr = ravenRepository.GetById(item.ProfileId);
                        if (pr != null)
                            ravenRepository.Delete(pr);
                    }
                    var profile = _profileService.GetProfile(item.ProfileId);
                    if(profile != null)
                        ravenRepository.UpdateStatus(Mapper.Map<State>(profile));
                } catch (Exception ex) {
                    result.Add(String.Format("{0} - {1}", item.ProfileId, ex.Message));
                }
            }
            return result;
        }
    }
}
