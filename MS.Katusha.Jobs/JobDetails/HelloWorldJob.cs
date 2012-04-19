using System;
using System.Diagnostics;
using System.Globalization;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Interfaces.Services;
using Quartz;

namespace MS.Katusha.Jobs.JobDetails
{
    [KatushaQuartzJob(ResourceString = "HelloWorldJob")]
    //[KatushaQuartzJob(Name = "HelloWorld", Group = "HelloWorldGroup", Interval = 30, JobName = "HelloWorldJob", JobGroup = "HelloWorldJobGroup")]
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
            Debug.WriteLine("Hello at " + profile.Name + DateTimeOffset.UtcNow.DateTime.ToLocalTime().ToString(CultureInfo.InvariantCulture));
        }
    }
    [KatushaQuartzJob(ResourceString = "HelloWorld2Job")]
    //[KatushaQuartzJob(Name = "HelloWorld", Group = "HelloWorldGroup", Interval = 30, JobName = "HelloWorldJob", JobGroup = "HelloWorldJobGroup")]
    public class HelloWorld2Job : IJob
    {
        private readonly IProfileService _profileService;

        public HelloWorld2Job(IProfileService profileService)
        {
            _profileService = profileService;
        }

        public void Execute(IJobExecutionContext context)
        {
            var profile = _profileService.GetProfile(1);
            Debug.WriteLine("Hello2 at " + profile.Name + DateTimeOffset.UtcNow.DateTime.ToLocalTime().ToString(CultureInfo.InvariantCulture));
        }
    }  
}
