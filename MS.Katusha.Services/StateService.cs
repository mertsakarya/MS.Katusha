using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class StateService : IStateService
    {
        private readonly IStateRepositoryDB _stateRepository;
        private readonly IStateRepositoryRavenDB _stateRepositoryRaven;
        private readonly IVisitService _visitService;
        private static readonly TimeSpan OnlineInterval = new TimeSpan(0, 0, 10, 0); 

        public StateService(IStateRepositoryDB stateRepository, IStateRepositoryRavenDB stateRepositoryRaven, IVisitService visitService)
        {
            _stateRepository = stateRepository;
            _stateRepositoryRaven = stateRepositoryRaven;
            _visitService = visitService;
        }

        public NewVisits Ping(long profileId, Sex gender) {
            var lastVisitTime = _stateRepository.UpdateStatus(profileId, gender);
            _stateRepositoryRaven.UpdateStatus(profileId, gender);
            return _visitService.GetVisitorsSinceLastVisit(profileId, lastVisitTime);
        }

        public bool IsOnline(long profileId) { 
            var state = _stateRepositoryRaven.GetById(profileId);
            var diff = DateTimeOffset.UtcNow - state.LastOnline;
            return ( diff < OnlineInterval);
        }

        public IEnumerable<State> OnlineGirls(out int total, int pageNo = 1, int pageSize = 20)
        {
            var dt = DateTimeOffset.UtcNow - OnlineInterval;
            var retval = _stateRepositoryRaven.Query(p => p.LastOnline > dt && p.Gender == (byte)Sex.Female, pageNo, pageSize, out total, p => p.LastOnline, false);
            //total = _stateRepositoryRaven.Count(p => p.LastOnline > dt && p.Gender == (byte)Sex.Female);
            return retval;
        }
        public IEnumerable<State> OnlineMen(out int total, int pageNo = 1, int pageSize = 20)
        {
            var dt = DateTimeOffset.UtcNow - OnlineInterval;
            var retval = _stateRepositoryRaven.Query(p => p.LastOnline > dt && p.Gender == (byte)Sex.Male, pageNo, pageSize, out total, p => p.LastOnline, false);
            //total = _stateRepositoryRaven.Count(p => p.LastOnline > dt && p.Gender == (byte)Sex.Male);
            return retval;
        }
        public IEnumerable<State> OnlineProfiles(out int total, int pageNo = 1, int pageSize = 20)
        {
            var dt = DateTimeOffset.UtcNow - OnlineInterval;
            var retval = _stateRepositoryRaven.Query(p => p.LastOnline > dt, pageNo, pageSize, out total, p => p.LastOnline, false);
            //total = _stateRepositoryRaven.Count(p => p.LastOnline > dt);
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
                    ravenRepository.UpdateStatus(item.ProfileId, (Sex)item.Gender);
                } catch (Exception ex) {
                    result.Add(String.Format("{0} - {1}", item.ProfileId, ex.Message));
                }
            }
            return result;
        }
    }
}
