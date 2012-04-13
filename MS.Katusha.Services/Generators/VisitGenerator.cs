using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Interfaces.Services;
using NLog;

namespace MS.Katusha.Services.Generators
{
    public class VisitGenerator : IGenerator<Visit>
    {
        private readonly IVisitService _visitService;
        private readonly List<Profile> _profiles;
        private int _total;

        public VisitGenerator(IProfileService profileService, IVisitService visitService)
        {
            _visitService = visitService;
            _profiles = new List<Profile>(profileService.GetNewProfiles(p => p.Id > 0, out _total, 1, 200));
            _total = _profiles.Count;

        }

        public Visit Generate(int extra = 0) { 
            if (extra > 0) _total = extra;
            var num = GeneratorHelper.RND.Next(_total - 2) + 1;
            var from = _profiles[num];
            num = GeneratorHelper.RND.Next(_total - 2) + 1;
            var to = _profiles[num];
            if (from.Id == to.Id) {
                return Generate(_total);
            } else {
                _visitService.Visit(from, to);
                return null;
            }
        }
    }
}