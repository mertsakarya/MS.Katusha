using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class StateService : IStateService
    {
        private readonly IStateRepositoryDB _stateRepository;
        private readonly IStateRepositoryRavenDB _stateRepositoryRaven;
        private static readonly TimeSpan OnlineInterval = new TimeSpan(0, 0, 10, 0); 

        public StateService(IStateRepositoryDB stateRepository, IStateRepositoryRavenDB stateRepositoryRaven)
        {
            _stateRepository = stateRepository;
            _stateRepositoryRaven = stateRepositoryRaven;
        }

        public void Ping(long profileId, Sex gender) {
            _stateRepository.UpdateStatus(profileId, gender);
            _stateRepositoryRaven.UpdateStatus(profileId, gender);
        }

        public bool IsOnline(long profileId) { 
            var state = _stateRepositoryRaven.GetById(profileId);
            var diff = DateTime.UtcNow - state.LastOnline;
            return ( diff < OnlineInterval);
        }

        public IEnumerable<State> OnlineGirls(out int total, int pageNo = 1, int pageSize = 20)
        {
            var dt = DateTime.UtcNow - OnlineInterval;
            return _stateRepositoryRaven.Query(p => p.LastOnline > dt && p.Gender == (byte)Sex.Female, pageNo, pageSize, out total, p => p.LastOnline, false);
        }
        public IEnumerable<State> OnlineMen(out int total, int pageNo = 1, int pageSize = 20)
        {
            var dt = DateTime.UtcNow - OnlineInterval;
            return _stateRepositoryRaven.Query(p => p.LastOnline > dt && p.Gender == (byte)Sex.Male, pageNo, pageSize, out total, p => p.LastOnline, false);
        }
        public IEnumerable<State> OnlineProfiles(out int total, int pageNo = 1, int pageSize = 20)
        {
            var dt = DateTime.UtcNow - OnlineInterval;
            return _stateRepositoryRaven.Query(p => p.LastOnline > dt, pageNo, pageSize, out total, p => p.LastOnline, false);
        }
    }
}
