using System;
using System.Diagnostics;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Interfaces.Services;
using Quartz;

namespace MS.Katusha.Jobs
{
    [KatushaQuartzJob(Name = "helloJob", Interval = 1)]
    public class HelloWorldJob : IJob
    {
        private readonly IProfileService _profileService;

        public HelloWorldJob(IProfileService profileService)
        {
            _profileService = profileService;
        }

        public void Execute(IJobExecutionContext context)
        {
            var profile = _profileService.GetProfile(1);
            Debug.WriteLine("Hello at " + profile.Name + DateTime.Now.ToString());
        }
    }

    [KatushaQuartzJob(Name = "hello2Job", Interval = 3)]
    public class HelloWorld2Job : IJob
    {
        private readonly IUserService _userService;
        private readonly IProfileService _profileService;

        public HelloWorld2Job(IUserService userService, IProfileService profileService)
        {
            _userService = userService;
            _profileService = profileService;
        }

        public void Execute(IJobExecutionContext context)
        {
            var profile = _profileService.GetProfile(1);
            Debug.WriteLine("Hello2 at " + profile.Name + DateTime.Now.ToString());
        }
    }
}
