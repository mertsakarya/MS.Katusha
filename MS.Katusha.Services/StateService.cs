using System;
using System.Collections.Generic;
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
        //private readonly IStateRepositoryDB _stateRepository;
        private readonly IVideoRoomService _videoRoomService;
        private readonly IStateRepositoryRavenDB _stateRepositoryRaven;
        private readonly IVisitService _visitService;
        private readonly IConversationService _conversationService;
        private static readonly TimeSpan OnlineInterval = new TimeSpan(0, 0, 10, 0);
        private readonly TokBox _tokBox;

        public StateService(IVideoRoomService videoRoomService, IStateRepositoryRavenDB stateRepositoryRaven, IVisitService visitService, IConversationService conversationService)
        {
            //_stateRepository = stateRepository;
            _videoRoomService = videoRoomService;
            _stateRepositoryRaven = stateRepositoryRaven;
            _visitService = visitService;
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

        public PingResult Ping(Profile profile)
        {
            if(profile == null) return null;
            try {
                var state = GetState(profile);
                var lastVisitTime = state.LastOnline; //GetDbState(profile);  //_stateRepository.UpdateStatus(profile);
                var visits = _visitService.GetVisitorsSinceLastVisit(profile.Id, lastVisitTime);
                var conversations = _conversationService.GetConversationStatistics(profile.Id);
                return new PingResult {Visits = visits, Conversations = conversations, State = state};
            } catch {
                return null;
            }
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

        public IList<VideoRoom> GetVideoRooms(Profile profile)
        {
            if(profile == null) return new List<VideoRoom>();
            return _videoRoomService.GetRooms(profile);
        }

        private void SetToxBoxInformation(State state)
        {
            var videoRoom = _videoRoomService.GetVideoRoom(VideoRoomNames.Everyone);
            state.TokBoxSessionId = videoRoom.TokBoxSessionId;
            if (!String.IsNullOrWhiteSpace(state.TokBoxTicketId) && DateTime.Now.AddHours(-22) < state.LastOnline) return;
            var connectionMetadata = new Dictionary<string, object> { { "Name", state.Name }, { "Guid", state.ProfileGuid }, { "PhotoGuid", state.PhotoGuid }, { "Gender", state.Gender }, { "Country", state.CountryCode }, { "Age", DateTime.Now.Year - state.BirthYear } };
            state.TokBoxTicketId = _tokBox.GenerateToken(state.TokBoxSessionId, connectionMetadata);
        }

    }
}
