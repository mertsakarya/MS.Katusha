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
        private readonly TokBox _tokBox;

        public StateService(IStateRepositoryDB stateRepository, IStateRepositoryRavenDB stateRepositoryRaven, IVisitService visitService, IProfileService profileService, IConversationService conversationService)
        {
            _stateRepository = stateRepository;
            _stateRepositoryRaven = stateRepositoryRaven;
            _visitService = visitService;
            _profileService = profileService;
            _conversationService = conversationService;
            _tokBox = new TokBox();

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
            var items = dbRepository.Query(filter, 1, int.MaxValue, out total, p => p.Id, true);
            foreach (var item in items) {
                try {
                    if (deleteIfExists) {
                        var pr = ravenRepository.GetState(item.ProfileId);
                        if (pr != null)
                            ravenRepository.Delete(pr);
                    }
                    var profile = _profileService.GetProfile(item.ProfileId);
                    if (profile != null)
                        ravenRepository.Update(Mapper.Map<State>(profile));
                } catch (Exception ex) {
                    result.Add(String.Format("{0} - {1}", item.ProfileId, ex.Message));
                }
            }
            return result;
        }

        public PingResult Ping(Profile profile)
        {
            if(profile == null) return null;
            try {
                var lastVisitTime = GetDbState(profile);  //_stateRepository.UpdateStatus(profile);
                var state = GetState(profile);
                var visits = _visitService.GetVisitorsSinceLastVisit(profile.Id, lastVisitTime);
                var conversations = _conversationService.GetConversationStatistics(profile.Id);
                return new PingResult {Visits = visits, Conversations = conversations, State = state};
            } catch {
                return null;
            }
        }
        private DateTime GetDbState(Profile profile)
        {
            var state = _stateRepository.GetState(profile.Id);
            DateTime retVal;
            if (state == null) {
                retVal = new DateTime(1900, 1, 1);
                state = Mapper.Map<State>(profile);
                state.LastOnline = DateTime.Now;
                _stateRepository.Add(state);
            } else {
                retVal = state.LastOnline;
                state.LastOnline = DateTime.Now;
                _stateRepository.Update(state);
            }
            return retVal;
        }

        public State GetState(Profile profile)
        {
            if (profile == null) return null;
            try {
                var state = _stateRepositoryRaven.GetState(profile) ?? Mapper.Map<State>(profile);
                SetToxBoxInformation(state);
                _stateRepositoryRaven.Update(state);
                return state;
            } catch {
                return null;
            }
        }

        private bool SetToxBoxInformation(State state)
        {
            var modified = false;
            var connectionMetadata = new Dictionary<string, object> { { "Name", state.Name }, { "Guid", state.ProfileGuid }, { "PhotoGuid", state.PhotoGuid }, { "Gender", state.Gender }, { "Country", state.CountryCode }, { "Age", DateTime.Now.Year - state.BirthYear } };
            if (String.IsNullOrWhiteSpace(state.TokBoxSessionId)) {
                state.TokBoxSessionId = _tokBox.CreateSession();
                state.TokBoxTicketId = _tokBox.GenerateToken(state.TokBoxSessionId, connectionMetadata);
                modified = true;
            } else
                if (!String.IsNullOrWhiteSpace(state.TokBoxTicketId) && DateTime.Now.AddHours(-22) < state.LastOnline) { } else {
                    state.TokBoxTicketId = _tokBox.GenerateToken(state.TokBoxSessionId, connectionMetadata);
                    modified = true;
                }
            return modified;
        }

    }
}
